# Development And Deployment

This document captures the backend development environment and the repeatable Cloud Run deployment process.

## Local Environment

Copy the sample environment file when you need custom local values:

```bash
cp .env.example .env
```

Set `JWT_SECRET` in the untracked `.env` file to a generated value of at least 32 bytes. Docker Compose uses only `http://localhost:5173` and `http://127.0.0.1:5173` when `CORS_ALLOWED_ORIGINS` is blank.

Start MySQL and the API with Docker Compose:

```bash
docker compose up --build
```

Local URLs:

```text
API: http://localhost:5271
Swagger UI: http://localhost:5271/swagger/index.html
Swagger JSON: http://localhost:5271/swagger/v1/swagger.json
```

The API container uses the MySQL service name from Docker Compose:

```text
server=mysql;port=3306;user=root;password=root;database=coldtrace_platform
```

If only the database is needed, start MySQL and run the API with the local SDK:

```bash
docker compose up -d mysql

JWT_SECRET="$(openssl rand -base64 48)" \
ASPNETCORE_ENVIRONMENT=Development \
/Users/mauriciopajes/.dotnet/dotnet run \
  --project coldtrace-platform/coldtrace-platform.csproj \
  --urls http://localhost:5271
```

## Continuous Integration

GitHub Actions runs the backend CI workflow on `main`, `develop`, `feature/**`, `release/**`, and pull requests into `main` or `develop`.

The workflow performs:

```text
dotnet restore
dotnet build --configuration Release
dotnet test when test projects exist
docker build
```

Workflow file:

```text
.github/workflows/backend-ci.yml
```

## Cloud Run Deployment

Production uses a Cloud SQL Auth Proxy sidecar. The API container connects to MySQL through local TCP at `127.0.0.1:3306`; Cloud SQL authorized networks are not required for the backend service.

Versioned deployment assets:

```text
deploy/cloud-run/service.template.yaml
scripts/deploy-cloud-run.sh
scripts/harden-gcp-runtime.sh
```

Required Google Cloud resources:

```text
Cloud Run service: coldtrace-platform
Cloud SQL instance: coldtrace-mysql
Secret Manager secret: coldtrace-db-password
Secret Manager secret: coldtrace-jwt-secret
Secret Manager secret: coldtrace-openai-api-key
Secret Manager secret: coldtrace-google-oauth-client-secret
Secret Manager secret: coldtrace-apple-private-key
Secret Manager secret: coldtrace-stripe-secret-key
Secret Manager secret: coldtrace-stripe-webhook-secret
Cloud Run service account: coldtrace-cloud-run-runtime@coldtrace-platform-20260619.iam.gserviceaccount.com
Project-level runtime role: roles/cloudsql.client
Secret-level runtime role on each required secret: roles/secretmanager.secretAccessor
Artifact Registry repository: cloud-run-source-deploy
```

Secret values are never rendered into the manifest. Create each secret, add its value as a Secret Manager version, and grant the dedicated Cloud Run service account access on that individual secret before replacing the service.

External provider setup must also exist outside this repository:

```text
Google: web OAuth client with the Vue production origin and sign-in redirect authorized
Apple: Service ID, production return URL, team id, key id, and Sign in with Apple private key
OpenAI: API key with access to the configured model
Stripe: products/prices, Customer Portal configuration, and a webhook for /api/v1/billing/stripe/webhooks
```

The Vue deployment may override the public browser values with `VITE_GOOGLE_OAUTH_CLIENT_ID`, `VITE_APPLE_OAUTH_CLIENT_ID`, and `VITE_APPLE_OAUTH_REDIRECT_URI`. These values are public identifiers; provider secrets stay only in the backend.

Run the runtime hardening apply workflow below once before the first deployment that uses the dedicated service account. The deployment script defaults to that identity and does not create it or grant IAM roles.

After exporting the required public provider configuration, deploy with explicit environment values:

```bash
GCP_PROJECT_ID=coldtrace-platform-20260619 \
GCP_REGION=us-central1 \
SERVICE_NAME=coldtrace-platform \
DB_INSTANCE_NAME=coldtrace-mysql \
DATABASE_NAME=coldtrace_platform \
DATABASE_USER=coldtrace_app \
DB_SECRET_NAME=coldtrace-db-password \
JWT_SECRET_NAME=coldtrace-jwt-secret \
JWT_EXPIRATION_DAYS=7 \
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app \
FRONTEND_ORIGIN=https://coldtrace-frontend-web.vercel.app \
GOOGLE_OAUTH_CLIENT_ID=your-google-web-client-id \
APPLE_OAUTH_CLIENT_ID=your-apple-service-id \
APPLE_TEAM_ID=your-apple-team-id \
APPLE_KEY_ID=your-apple-key-id \
AI_ASSISTANCE_ENABLED=true \
AI_MODEL_PROVIDER=openai \
AI_MODEL_NAME=gpt-5.4-mini \
STRIPE_OPERATIONS_PRICE_ID=price_operations \
STRIPE_COMPLIANCE_AI_PRICE_ID=price_compliance_ai \
CLOUD_RUN_SERVICE_ACCOUNT_NAME=coldtrace-cloud-run-runtime \
scripts/deploy-cloud-run.sh
```

By default, provider redirects and Stripe returns are derived from `FRONTEND_ORIGIN`. Billing returns use the canonical Vue route:

```text
/settings/billing?checkout=success&session_id={CHECKOUT_SESSION_ID}
/settings/billing?checkout=cancel
/settings/billing?portal=return
```

Use `DRY_RUN=true` with the same non-secret public configuration to inspect the manifest. The output contains Secret Manager names and references, never secret values. The script stops if a required public value or a template placeholder is missing.

`JWT_SECRET_NAME` identifies a Secret Manager secret; the JWT value itself is never placed in the manifest or deployment command. Production startup also fails closed when the JWT secret or `CORS_ALLOWED_ORIGINS` is missing.

The script builds the image tag from the project version in `coldtrace-platform/coldtrace-platform.csproj`. For version `1.0.2`, the default image tag is:

```text
us-central1-docker.pkg.dev/coldtrace-platform-20260619/cloud-run-source-deploy/coldtrace-platform/coldtrace-platform:v1.0.2
```

After deployment, the script checks:

```text
/api/v1/subscription-plans
```

## Cloud Runtime Security

The TS35 tool is restricted to project `coldtrace-platform-20260619` (project number `55771439812`). Its defaults are:

```text
Region: us-central1
Cloud Run service: coldtrace-platform
Cloud SQL instance: coldtrace-mysql
Runtime service account: coldtrace-cloud-run-runtime
Database secret: coldtrace-db-password
Backup start time when backups are disabled: 03:00 UTC
```

Run the default audit at any time:

```bash
scripts/harden-gcp-runtime.sh
```

The audit uses read-only `gcloud describe` and IAM policy commands. It reports authorized networks, SSL mode, deletion protection, automated backup and MySQL binary-log/PITR state, the Cloud Run service identity, project roles for the current and dedicated identities, and access to the database secret. It never accesses secret versions or prints secret values.

### Apply Safeguards

Apply mode requires `APPLY=true`, an exact `CONFIRM_PROJECT`, and an interactive target phrase:

```bash
APPLY=true \
CONFIRM_PROJECT=coldtrace-platform-20260619 \
scripts/harden-gcp-runtime.sh
```

At the prompt, type:

```text
APPLY coldtrace-platform-20260619/us-central1/coldtrace-platform/coldtrace-mysql
```

For a reviewed non-interactive run, provide the same phrase explicitly:

```bash
APPLY=true \
CONFIRM_PROJECT=coldtrace-platform-20260619 \
CONFIRM_APPLY='APPLY coldtrace-platform-20260619/us-central1/coldtrace-platform/coldtrace-mysql' \
scripts/harden-gcp-runtime.sh
```

Apply mode performs only these changes:

```text
Create or reuse the dedicated runtime service account.
Grant roles/cloudsql.client at project level.
Grant roles/secretmanager.secretAccessor on coldtrace-db-password only.
Clear Cloud SQL authorized networks because the runtime uses Cloud SQL Auth Proxy.
Set the Cloud SQL SSL mode to ENCRYPTED_ONLY.
Enable Cloud SQL deletion protection and automated backups when disabled.
Enable MySQL binary logging for point-in-time recovery.
Deploy a Cloud Run revision that uses the dedicated runtime identity.
```

The tool refuses a different project or project number and refuses to use a dedicated account that already has project roles other than `roles/cloudsql.client`. It does not remove permissions from the previous default Compute Engine account. Enforcing SSL or enabling binary logging can restart Cloud SQL, so use a reviewed maintenance window.

### Per-Secret Access

Never grant `roles/secretmanager.secretAccessor` to the runtime account at project level. When a provider secret is added later, grant access on that individual secret after reviewing its use:

```bash
SECRET_NAME=PROVIDER_SECRET_NAME

gcloud secrets add-iam-policy-binding "$SECRET_NAME" \
  --project coldtrace-platform-20260619 \
  --member='serviceAccount:coldtrace-cloud-run-runtime@coldtrace-platform-20260619.iam.gserviceaccount.com' \
  --role='roles/secretmanager.secretAccessor' \
  --condition=None
```

Do not place provider credential values in `.env.example`, deployment manifests, command output, or repository files.

### Security Smoke Checks

After apply mode completes, rerun the audit and verify that the latest ready revision uses the dedicated identity. Then check the ASP.NET Core endpoint without printing its response body:

```bash
scripts/harden-gcp-runtime.sh

SERVICE_URL="$(gcloud run services describe coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1 \
  --format='value(status.url)')"

curl -fsS -o /dev/null -w 'HTTP %{http_code}\n' \
  "${SERVICE_URL}/api/v1/subscription-plans"
```

The expected status is `HTTP 200`. Inspect recent startup and Cloud SQL Auth Proxy logs for permission or connection errors:

```bash
gcloud run services logs read coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1 \
  --limit 100
```

The audit should show no authorized networks, `ENCRYPTED_ONLY`, deletion protection and backups enabled, MySQL binary logging enabled, only `roles/cloudsql.client` at project level for the dedicated identity, and Secret Accessor only on explicitly listed secrets.

### Security Rollback

Before apply, record the current identity and latest ready revision from the audit output. If the new revision fails, move traffic back first:

```bash
gcloud run revisions list \
  --service coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1

gcloud run services update-traffic coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1 \
  --to-revisions PREVIOUS_READY_REVISION=100
```

Cloud SQL Auth Proxy supports the encrypted connection requirement and does not need authorized networks. Do not restore `0.0.0.0/0` to troubleshoot a failed revision. Check the dedicated account's Cloud SQL Client and per-secret database access first.

After traffic is stable on the previous revision and no active revision needs the dedicated identity, its grants can be removed:

```bash
gcloud secrets remove-iam-policy-binding coldtrace-db-password \
  --project coldtrace-platform-20260619 \
  --member='serviceAccount:coldtrace-cloud-run-runtime@coldtrace-platform-20260619.iam.gserviceaccount.com' \
  --role='roles/secretmanager.secretAccessor' \
  --condition=None

gcloud projects remove-iam-policy-binding coldtrace-platform-20260619 \
  --member='serviceAccount:coldtrace-cloud-run-runtime@coldtrace-platform-20260619.iam.gserviceaccount.com' \
  --role='roles/cloudsql.client' \
  --condition=None
```

Keep deletion protection, backups, PITR, encrypted connections, and closed authorized networks enabled during a Cloud Run rollback.

## Database Access

For local database access to Cloud SQL, use Cloud SQL Auth Proxy instead of opening broad authorized networks:

```bash
cloud-sql-proxy coldtrace-platform-20260619:us-central1:coldtrace-mysql --port 3306
```

Then connect the MySQL client to:

```text
Host: 127.0.0.1
Port: 3306
Database: coldtrace_platform
User: coldtrace_app
Password: value stored in Secret Manager
```

## Rollback

List revisions:

```bash
gcloud run revisions list \
  --service coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1
```

Move traffic back to a previous ready revision:

```bash
gcloud run services update-traffic coldtrace-platform \
  --project coldtrace-platform-20260619 \
  --region us-central1 \
  --to-revisions REVISION_NAME=100
```
