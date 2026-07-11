# ColdTrace Platform

ASP.NET Core Web API for the ColdTrace cold-chain monitoring platform.

The backend exposes organization-scoped REST endpoints for IAM, asset management, telemetry, alerts, maintenance operations, and reports. Swagger/OpenAPI is enabled for course delivery, manual validation, and frontend integration.

## Stack

- C# 14
- ASP.NET Core Web API targeting `net10.0`
- Entity Framework Core 10
- MySQL through `MySql.EntityFrameworkCore`
- Swagger/OpenAPI through Swashbuckle annotations
- ASP.NET Core JWT bearer authentication with HS256 validation
- Resource-based localization for `en` and `es`
- Microsoft.Extensions.AI abstractions for provider-neutral AI features

## Backend Scope

| Bounded context | Main capabilities |
| --- | --- |
| IAM | Organizations, organization sign-up, users, roles, permissions |
| Asset Management | Locations, gateways, assets, asset settings, IoT devices |
| Monitoring | Sensor readings, range evaluation, demo telemetry generation |
| Alerts | Incidents, acknowledgements, escalation, corrective action, AI resolution plans, resolutions, notifications |
| Maintenance Management | Maintenance schedules and technical service requests |
| Reports | Operational report generation and report lookup |
| AI Assistance | Provider configuration, structured output contracts for dashboard interpretation, report AI summaries, incident AI resolution plans, and AI diagnostics |

## Repository Layout

```text
coldtrace-platform/
  Alerts/
  AssetManagement/
  Iam/
  MaintenanceManagement/
  Monitoring/
  Reports/
  AiAssistance/
  Shared/
  Migrations/
docs/
bruno/
deploy/
scripts/
.github/workflows/
```

Each bounded context follows the same high-level structure:

```text
Application/
  CommandServices/
  Internal/
  QueryServices/
Domain/
Infrastructure/Persistence/EntityFrameworkCore/Repositories/
Interfaces/Rest/
Resources/
```

## Local Setup

Use the .NET SDK installed under `~/.dotnet/dotnet`. On this machine, the system `dotnet` under `/usr/local/share/dotnet` may not have the required SDK for `net10.0`.

```bash
/Users/mauriciopajes/.dotnet/dotnet restore coldtrace-platform/coldtrace-platform.csproj
/Users/mauriciopajes/.dotnet/dotnet build coldtrace-platform/coldtrace-platform.csproj
```

The development connection string is configured in:

```text
coldtrace-platform/appsettings.Development.json
```

Default local MySQL credentials:

```text
server=localhost;user=root;password=root;database=coldtrace_platform
```

If your local password is different, override the connection string through an environment variable instead of committing personal credentials:

```bash
ConnectionStrings__DefaultConnection="server=localhost;user=root;password=<your-password>;database=coldtrace_platform"
```

Run the API locally:

```bash
JWT_SECRET="$(openssl rand -base64 48)" \
ASPNETCORE_ENVIRONMENT=Development \
/Users/mauriciopajes/.dotnet/dotnet run \
  --project coldtrace-platform/coldtrace-platform.csproj \
  --urls http://localhost:5271
```

Swagger UI:

```text
http://localhost:5271/swagger/index.html
```

Swagger JSON:

```text
http://localhost:5271/swagger/v1/swagger.json
```

For a complete Docker-based environment, run:

```bash
cp .env.example .env
docker compose up --build
```

See [docs/DEVELOPMENT_AND_DEPLOYMENT.md](docs/DEVELOPMENT_AND_DEPLOYMENT.md) for the local environment, CI workflow, Cloud Run deployment script, and rollback process.

## Database

The application creates the configured MySQL database if it does not exist, ensures the EF migrations history table exists, and applies pending migrations on startup.

In Cloud Run, production connects to Cloud SQL through a Cloud SQL Auth Proxy sidecar. The API container connects to the proxy over local TCP, so the deployed service does not depend on the database public IP.

```text
DATABASE_URL=127.0.0.1
DATABASE_SCHEMA=coldtrace_platform
DATABASE_USER=coldtrace_app
DATABASE_PASSWORD=<from Secret Manager>
JWT_SECRET_NAME=coldtrace-jwt-secret
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app,http://localhost:5173,http://127.0.0.1:5173
AI_ASSISTANCE_ENABLED=false
AI_MODEL_PROVIDER=disabled
AI_MODEL_NAME=
OLLAMA_BASE_URL=http://localhost:11434
OPENAI_API_KEY=<from Secret Manager when enabled>
AI_REQUEST_TIMEOUT=30s
```

`CORS_ALLOWED_ORIGINS` is a comma-separated exact allowlist for browser clients. Development defaults to the two Vite origins when it is omitted; every other environment requires it. The Cloud Run deployment default keeps the stable Vercel production domain plus `http://localhost:5173` and `http://127.0.0.1:5173`, allowing the local Vue application to test against the deployed API without enabling arbitrary origins.

API controllers require a valid bearer token by default. The explicit anonymous routes are sign-in, first-tenant organization sign-up, subscription plan catalog reads, and the signed Stripe webhook. Swagger/OpenAPI assets remain public for course validation.

The Cloud Run service account must have `roles/cloudsql.client` and `roles/secretmanager.secretAccessor`. The sidecar runs `gcr.io/cloud-sql-connectors/cloud-sql-proxy:2` against the Cloud SQL instance connection name and exposes MySQL locally on port `3306`.

The Cloud Run manifest is versioned as a template in:

```text
deploy/cloud-run/service.template.yaml
```

Deployment is automated through:

```bash
scripts/deploy-cloud-run.sh
```

The current schema contains 22 domain tables plus EF Core's `__EFMigrationsHistory` table:

```text
ai_resolution_plan_required_evidences
ai_resolution_plan_steps
ai_resolution_plan_uncertainty_notes
ai_resolution_plans
asset_settings
asset_settings_asset_types
asset_settings_iot_device_types
assets
gateways
incidents
iot_device_measurement_parameters
iot_devices
locations
maintenance_schedules
notifications
organizations
reports
role_permissions
roles
sensor_readings
technical_service_requests
users
```

## Endpoint Groups

All organization-owned operational resources are scoped under `/api/v1/organizations/{organizationId}`.

| Area | Routes |
| --- | --- |
| Organizations | `/api/v1/organizations`, `/api/v1/organization-sign-ups` |
| Roles and users | `/api/v1/roles`, `/api/v1/organizations/{organizationId}/users` |
| Assets | `/api/v1/organizations/{organizationId}/locations`, `/gateways`, `/assets`, `/iot-devices`, `/asset-settings` |
| Monitoring | `/api/v1/organizations/{organizationId}/sensor-readings` |
| Alerts | `/api/v1/organizations/{organizationId}/incidents`, `/incidents/{incidentId}/ai-resolution-plans`, `/incidents/{incidentId}/ai-resolution-plans/{planId}/approvals`, `/incidents/{incidentId}/ai-resolution-plans/{planId}/rejections`, `/notifications` |
| Maintenance | `/api/v1/organizations/{organizationId}/maintenance-schedules`, `/technical-service-requests` |
| Reports | `/api/v1/organizations/{organizationId}/reports` |
| AI Assistance | `/api/v1/ai-assistance/provider-status` |

Plan-limited writes and paid AI operations are enforced server-side before persistence. When the current subscription plan does not allow the operation, the API returns `409 Conflict` with RFC 7807 `ProblemDetails` and the same entitlement metadata exposed by the Spring Boot implementation:

```text
organizationId
planCode
subscriptionStatus
entitlementKey
entitlementCategory
entitlementEnabled
limit
used
remaining
lockedReason
requiredPlanCode
```

Current backend checks cover organization-scoped creation of locations, assets, IoT devices, users, reports, maintenance schedules, technical service requests, AI incident guidance, and AI report summaries.

See [docs/API_REFERENCE.md](docs/API_REFERENCE.md) for the current route and payload reference.

## Validation

The latest backend verification covered build, database migration, seeded data parity, and a 53-request smoke flow across the main endpoint workflows.

Core commands:

```bash
/Users/mauriciopajes/.dotnet/dotnet build coldtrace-platform/coldtrace-platform.csproj
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app DRY_RUN=true scripts/deploy-cloud-run.sh
```

GitHub Actions also runs restore, release build, and Docker image build through [.github/workflows/backend-ci.yml](.github/workflows/backend-ci.yml).

## Branching

- `main`: stable branch
- `develop`: integration branch
- `feature/*`: scoped implementation branches

Use conventional commits for repository history.
