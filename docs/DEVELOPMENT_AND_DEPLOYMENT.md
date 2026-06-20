# Development And Deployment

This document captures the backend development environment and the repeatable Cloud Run deployment process.

## Local Environment

Copy the sample environment file when you need custom local values:

```bash
cp .env.example .env
```

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
Cloud Run service account role: roles/cloudsql.client
Artifact Registry repository: cloud-run-source-deploy
```

Deploy with defaults:

```bash
scripts/deploy-cloud-run.sh
```

Preview the rendered Cloud Run manifest without deploying:

```bash
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
scripts/deploy-cloud-run.sh
```

The script builds the image tag from the project version in `coldtrace-platform/coldtrace-platform.csproj`. For version `1.0.2`, the default image tag is:

```text
us-central1-docker.pkg.dev/coldtrace-platform-20260619/cloud-run-source-deploy/coldtrace-platform/coldtrace-platform:v1.0.2
```

After deployment, the script checks:

```text
/api/v1/roles
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
