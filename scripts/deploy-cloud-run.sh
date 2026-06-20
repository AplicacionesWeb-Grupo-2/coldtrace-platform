#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

require_command() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1" >&2
    exit 1
  fi
}

require_command gcloud
require_command perl

GCP_PROJECT_ID="${GCP_PROJECT_ID:-coldtrace-platform-20260619}"
GCP_REGION="${GCP_REGION:-us-central1}"
SERVICE_NAME="${SERVICE_NAME:-coldtrace-platform}"
DB_INSTANCE_NAME="${DB_INSTANCE_NAME:-coldtrace-mysql}"
DATABASE_NAME="${DATABASE_NAME:-coldtrace_platform}"
DATABASE_USER="${DATABASE_USER:-coldtrace_app}"
DB_SECRET_NAME="${DB_SECRET_NAME:-coldtrace-db-password}"
MAX_INSTANCES="${MAX_INSTANCES:-1}"
CORS_ALLOWED_ORIGINS="${CORS_ALLOWED_ORIGINS:-https://coldtrace-frontend-web.vercel.app,https://coldtrace-frontend-q1gkddcns-mauricio-pajes-projects.vercel.app,http://localhost:5173}"
BUILD_IMAGE="${BUILD_IMAGE:-true}"
DRY_RUN="${DRY_RUN:-false}"
SMOKE_ENDPOINT_PATH="${SMOKE_ENDPOINT_PATH:-/api/v1/roles}"

APP_VERSION="$(
  sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' coldtrace-platform/coldtrace-platform.csproj | head -n 1
)"
if [[ -z "$APP_VERSION" ]]; then
  APP_VERSION="dev-$(git rev-parse --short HEAD)"
fi

GCP_PROJECT_NUMBER="${GCP_PROJECT_NUMBER:-$(gcloud projects describe "$GCP_PROJECT_ID" --format='value(projectNumber)')}"
CLOUD_RUN_SERVICE_ACCOUNT="${CLOUD_RUN_SERVICE_ACCOUNT:-${GCP_PROJECT_NUMBER}-compute@developer.gserviceaccount.com}"
DB_INSTANCE_CONNECTION_NAME="${DB_INSTANCE_CONNECTION_NAME:-$(gcloud sql instances describe "$DB_INSTANCE_NAME" --project "$GCP_PROJECT_ID" --format='value(connectionName)')}"
IMAGE_REPOSITORY="${IMAGE_REPOSITORY:-${GCP_REGION}-docker.pkg.dev/${GCP_PROJECT_ID}/cloud-run-source-deploy/coldtrace-platform/coldtrace-platform}"
IMAGE_URI="${IMAGE_URI:-${IMAGE_REPOSITORY}:v${APP_VERSION}}"

export GCP_PROJECT_ID
export GCP_PROJECT_NUMBER
export GCP_REGION
export SERVICE_NAME
export DB_INSTANCE_CONNECTION_NAME
export DATABASE_NAME
export DATABASE_USER
export DB_SECRET_NAME
export MAX_INSTANCES
export CORS_ALLOWED_ORIGINS
export CLOUD_RUN_SERVICE_ACCOUNT
export IMAGE_URI

TEMPLATE="deploy/cloud-run/service.template.yaml"
if [[ ! -f "$TEMPLATE" ]]; then
  echo "Missing Cloud Run template: $TEMPLATE" >&2
  exit 1
fi

WORK_DIR="$(mktemp -d)"
trap 'rm -rf "$WORK_DIR"' EXIT
RENDERED_MANIFEST="$WORK_DIR/service.yaml"

perl -pe 's/\$\{([A-Z0-9_]+)\}/exists $ENV{$1} ? $ENV{$1} : $&/ge' \
  "$TEMPLATE" > "$RENDERED_MANIFEST"

if [[ "$DRY_RUN" == "true" ]]; then
  cat "$RENDERED_MANIFEST"
  exit 0
fi

if [[ "$BUILD_IMAGE" == "true" ]]; then
  gcloud builds submit --project "$GCP_PROJECT_ID" --tag "$IMAGE_URI"
fi

gcloud run services replace "$RENDERED_MANIFEST" \
  --project "$GCP_PROJECT_ID" \
  --region "$GCP_REGION"

SERVICE_URL="$(gcloud run services describe "$SERVICE_NAME" \
  --project "$GCP_PROJECT_ID" \
  --region "$GCP_REGION" \
  --format='value(status.url)')"

for attempt in {1..12}; do
  status_code="$(curl -sS -o "$WORK_DIR/smoke-response.json" -w '%{http_code}' "${SERVICE_URL}${SMOKE_ENDPOINT_PATH}" || true)"
  if [[ "$status_code" == "200" ]]; then
    echo "Deployment verified: ${SERVICE_URL}${SMOKE_ENDPOINT_PATH}"
    exit 0
  fi
  echo "Smoke check attempt ${attempt}/12 returned ${status_code:-no response}; retrying..."
  sleep 5
done

echo "Deployment finished, but smoke check did not return 200." >&2
echo "Service URL: $SERVICE_URL" >&2
exit 1
