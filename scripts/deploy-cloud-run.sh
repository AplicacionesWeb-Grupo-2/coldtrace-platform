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
JWT_SECRET_NAME="${JWT_SECRET_NAME:-coldtrace-jwt-secret}"
JWT_EXPIRATION_DAYS="${JWT_EXPIRATION_DAYS:-7}"
CLOUD_RUN_SERVICE_ACCOUNT_NAME="${CLOUD_RUN_SERVICE_ACCOUNT_NAME:-coldtrace-cloud-run-runtime}"
MAX_INSTANCES="${MAX_INSTANCES:-1}"
FRONTEND_ORIGIN="${FRONTEND_ORIGIN:-https://coldtrace-frontend-web.vercel.app}"
CORS_ALLOWED_ORIGINS="${CORS_ALLOWED_ORIGINS:-${FRONTEND_ORIGIN}}"
AI_ASSISTANCE_ENABLED="${AI_ASSISTANCE_ENABLED:-false}"
AI_MODEL_PROVIDER="${AI_MODEL_PROVIDER:-disabled}"
AI_MODEL_NAME="${AI_MODEL_NAME:-}"
OLLAMA_BASE_URL="${OLLAMA_BASE_URL:-http://localhost:11434}"
OPENAI_API_KEY_SECRET_NAME="${OPENAI_API_KEY_SECRET_NAME:-coldtrace-openai-api-key}"
AI_REQUEST_TIMEOUT="${AI_REQUEST_TIMEOUT:-30s}"
GOOGLE_OAUTH_CLIENT_ID="${GOOGLE_OAUTH_CLIENT_ID:-}"
GOOGLE_OAUTH_REDIRECT_URI="${GOOGLE_OAUTH_REDIRECT_URI:-${FRONTEND_ORIGIN}/identity-access/sign-in}"
GOOGLE_OAUTH_CLIENT_SECRET_NAME="${GOOGLE_OAUTH_CLIENT_SECRET_NAME:-coldtrace-google-oauth-client-secret}"
APPLE_OAUTH_CLIENT_ID="${APPLE_OAUTH_CLIENT_ID:-}"
APPLE_OAUTH_REDIRECT_URI="${APPLE_OAUTH_REDIRECT_URI:-${FRONTEND_ORIGIN}/identity-access/sign-in}"
APPLE_TEAM_ID="${APPLE_TEAM_ID:-}"
APPLE_KEY_ID="${APPLE_KEY_ID:-}"
APPLE_PRIVATE_KEY_SECRET_NAME="${APPLE_PRIVATE_KEY_SECRET_NAME:-coldtrace-apple-private-key}"
STRIPE_OPERATIONS_PRICE_ID="${STRIPE_OPERATIONS_PRICE_ID:-}"
STRIPE_COMPLIANCE_AI_PRICE_ID="${STRIPE_COMPLIANCE_AI_PRICE_ID:-}"
STRIPE_SECRET_KEY_SECRET_NAME="${STRIPE_SECRET_KEY_SECRET_NAME:-coldtrace-stripe-secret-key}"
STRIPE_WEBHOOK_SECRET_NAME="${STRIPE_WEBHOOK_SECRET_NAME:-coldtrace-stripe-webhook-secret}"
BILLING_CHECKOUT_SUCCESS_URL="${BILLING_CHECKOUT_SUCCESS_URL:-${FRONTEND_ORIGIN}/settings/billing?checkout=success&session_id={CHECKOUT_SESSION_ID}}"
BILLING_CHECKOUT_CANCEL_URL="${BILLING_CHECKOUT_CANCEL_URL:-${FRONTEND_ORIGIN}/settings/billing?checkout=cancel}"
BILLING_CUSTOMER_PORTAL_RETURN_URL="${BILLING_CUSTOMER_PORTAL_RETURN_URL:-${FRONTEND_ORIGIN}/settings/billing?portal=return}"
BUILD_IMAGE="${BUILD_IMAGE:-true}"
DRY_RUN="${DRY_RUN:-false}"
SMOKE_ENDPOINT_PATH="${SMOKE_ENDPOINT_PATH:-/api/v1/subscription-plans}"

if [[ -z "$CORS_ALLOWED_ORIGINS" ]]; then
  echo "CORS_ALLOWED_ORIGINS must contain the exact production browser origins." >&2
  exit 1
fi

required_configuration=(
  CORS_ALLOWED_ORIGINS
  GOOGLE_OAUTH_CLIENT_ID
  GOOGLE_OAUTH_REDIRECT_URI
  APPLE_OAUTH_CLIENT_ID
  APPLE_OAUTH_REDIRECT_URI
  APPLE_TEAM_ID
  APPLE_KEY_ID
  STRIPE_OPERATIONS_PRICE_ID
  STRIPE_COMPLIANCE_AI_PRICE_ID
  BILLING_CHECKOUT_SUCCESS_URL
  BILLING_CHECKOUT_CANCEL_URL
  BILLING_CUSTOMER_PORTAL_RETURN_URL
)

if [[ "$AI_ASSISTANCE_ENABLED" == "true" ]]; then
  required_configuration+=(AI_MODEL_PROVIDER AI_MODEL_NAME)
fi

for variable_name in "${required_configuration[@]}"; do
  if [[ -z "${!variable_name}" ]]; then
    echo "Missing required deployment configuration: ${variable_name}" >&2
    exit 1
  fi
done

APP_VERSION="$(
  sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' coldtrace-platform/coldtrace-platform.csproj | head -n 1
)"
if [[ -z "$APP_VERSION" ]]; then
  APP_VERSION="dev-$(git rev-parse --short HEAD)"
fi

GCP_PROJECT_NUMBER="${GCP_PROJECT_NUMBER:-$(gcloud projects describe "$GCP_PROJECT_ID" --format='value(projectNumber)')}"
CLOUD_RUN_SERVICE_ACCOUNT="${CLOUD_RUN_SERVICE_ACCOUNT:-${CLOUD_RUN_SERVICE_ACCOUNT_NAME}@${GCP_PROJECT_ID}.iam.gserviceaccount.com}"
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
export JWT_SECRET_NAME
export JWT_EXPIRATION_DAYS
export MAX_INSTANCES
export FRONTEND_ORIGIN
export CORS_ALLOWED_ORIGINS
export AI_ASSISTANCE_ENABLED
export AI_MODEL_PROVIDER
export AI_MODEL_NAME
export OLLAMA_BASE_URL
export OPENAI_API_KEY_SECRET_NAME
export AI_REQUEST_TIMEOUT
export GOOGLE_OAUTH_CLIENT_ID
export GOOGLE_OAUTH_REDIRECT_URI
export GOOGLE_OAUTH_CLIENT_SECRET_NAME
export APPLE_OAUTH_CLIENT_ID
export APPLE_OAUTH_REDIRECT_URI
export APPLE_TEAM_ID
export APPLE_KEY_ID
export APPLE_PRIVATE_KEY_SECRET_NAME
export STRIPE_OPERATIONS_PRICE_ID
export STRIPE_COMPLIANCE_AI_PRICE_ID
export STRIPE_SECRET_KEY_SECRET_NAME
export STRIPE_WEBHOOK_SECRET_NAME
export BILLING_CHECKOUT_SUCCESS_URL
export BILLING_CHECKOUT_CANCEL_URL
export BILLING_CUSTOMER_PORTAL_RETURN_URL
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

if grep -Eq '\$\{[A-Z0-9_]+\}' "$RENDERED_MANIFEST"; then
  echo "Rendered Cloud Run manifest contains unresolved configuration placeholders." >&2
  exit 1
fi

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
