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
```

Required Google Cloud resources:

```text
Cloud Run service: coldtrace-platform
Cloud SQL instance: coldtrace-mysql
Secret Manager secret: coldtrace-db-password
Secret Manager secret: coldtrace-jwt-secret
Cloud Run service account roles: roles/cloudsql.client and roles/secretmanager.secretAccessor
Artifact Registry repository: cloud-run-source-deploy
```

Deploy with an explicit production browser allowlist:

```bash
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app \
scripts/deploy-cloud-run.sh
```

Preview the rendered Cloud Run manifest without deploying:

```bash
CORS_ALLOWED_ORIGINS=https://coldtrace-frontend-web.vercel.app \
DRY_RUN=true scripts/deploy-cloud-run.sh
```

Override deployment values through environment variables:

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
scripts/deploy-cloud-run.sh
```

`JWT_SECRET_NAME` identifies a Secret Manager secret; the JWT value itself is never placed in the manifest or deployment command. Production startup also fails closed when the JWT secret or `CORS_ALLOWED_ORIGINS` is missing.

The script builds the image tag from the project version in `coldtrace-platform/coldtrace-platform.csproj`. For version `1.0.2`, the default image tag is:

```text
us-central1-docker.pkg.dev/coldtrace-platform-20260619/cloud-run-source-deploy/coldtrace-platform/coldtrace-platform:v1.0.2
```

After deployment, the script checks:

```text
/api/v1/subscription-plans
```

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
