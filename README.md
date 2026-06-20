# ColdTrace Platform

ASP.NET Core Web API for the ColdTrace cold-chain monitoring platform.

The backend exposes organization-scoped REST endpoints for identity access, asset management, telemetry, alerts, maintenance operations, and reports. Swagger/OpenAPI is enabled for course delivery, manual validation, and frontend integration.

## Stack

- C# 14
- ASP.NET Core Web API targeting `net10.0`
- Entity Framework Core 10
- MySQL through `MySql.EntityFrameworkCore`
- Swagger/OpenAPI through Swashbuckle annotations
- Resource-based localization for `en`, `en-US`, `es`, and `es-PE`

## Backend Scope

| Bounded context | Main capabilities |
| --- | --- |
| Identity Access | Organizations, organization sign-up, users, roles, permissions |
| Asset Management | Locations, gateways, assets, asset settings, IoT devices |
| Monitoring | Sensor readings, range evaluation, demo telemetry generation |
| Alerts | Incidents, acknowledgements, escalation, corrective action, resolutions, notifications |
| Maintenance Management | Maintenance schedules and technical service requests |
| Reports | Operational report generation and report lookup |

## Repository Layout

```text
coldtrace-platform/
  Alerts/
  AssetManagement/
  IdentityAccess/
  MaintenanceManagement/
  Monitoring/
  Reports/
  Resources/
  Shared/
docs/
bruno/
deploy/
scripts/
.github/workflows/
```

Each bounded context follows the same high-level structure:

```text
Application/
Domain/
Infrastructure/Persistence/EFC/Repositories/
Interfaces/REST/
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
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app,https://coldtrace-frontend-q1gkddcns-mauricio-pajes-projects.vercel.app,http://localhost:5173
```

`CORS_ALLOWED_ORIGINS` is a comma-separated allowlist for browser clients. Keep the stable Vercel production domain in the list and add temporary deployment URLs only when they must be tested directly.

The Cloud Run service account must have `roles/cloudsql.client`. The sidecar runs `gcr.io/cloud-sql-connectors/cloud-sql-proxy:2` against the Cloud SQL instance connection name and exposes MySQL locally on port `3306`.

The Cloud Run manifest is versioned as a template in:

```text
deploy/cloud-run/service.template.yaml
```

Deployment is automated through:

```bash
scripts/deploy-cloud-run.sh
```

The current schema contains 18 domain tables plus EF Core's `__EFMigrationsHistory` table:

```text
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
| Alerts | `/api/v1/organizations/{organizationId}/incidents`, `/notifications` |
| Maintenance | `/api/v1/organizations/{organizationId}/maintenance-schedules`, `/technical-service-requests` |
| Reports | `/api/v1/organizations/{organizationId}/reports` |

See [docs/API_REFERENCE.md](docs/API_REFERENCE.md) for the current route and payload reference.

## Validation

The latest backend verification covered build, database migration, seeded data parity, and a 53-request smoke flow across the main endpoint workflows.

Core commands:

```bash
/Users/mauriciopajes/.dotnet/dotnet build coldtrace-platform/coldtrace-platform.csproj
DRY_RUN=true scripts/deploy-cloud-run.sh
```

See [docs/SMOKE_TESTING.md](docs/SMOKE_TESTING.md) for the current smoke flow and [docs](docs) for ticket-level manual checklists.

GitHub Actions also runs restore, release build, optional tests, and Docker image build through [.github/workflows/backend-ci.yml](.github/workflows/backend-ci.yml).

## Branching

- `main`: stable branch
- `develop`: integration branch
- `feature/*`: scoped implementation branches

Use conventional commits for repository history.
