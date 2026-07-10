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

readonly TARGET_PROJECT_ID="coldtrace-platform-20260619"
readonly TARGET_PROJECT_NUMBER="55771439812"

GCP_PROJECT_ID="${GCP_PROJECT_ID:-$TARGET_PROJECT_ID}"
GCP_REGION="${GCP_REGION:-us-central1}"
SERVICE_NAME="${SERVICE_NAME:-coldtrace-platform}"
DB_INSTANCE_NAME="${DB_INSTANCE_NAME:-coldtrace-mysql}"
DB_SECRET_NAME="${DB_SECRET_NAME:-coldtrace-db-password}"
CLOUD_RUN_SERVICE_ACCOUNT_NAME="${CLOUD_RUN_SERVICE_ACCOUNT_NAME:-coldtrace-cloud-run-runtime}"
CLOUD_SQL_BACKUP_START_TIME="${CLOUD_SQL_BACKUP_START_TIME:-03:00}"
APPLY="${APPLY:-false}"
CONFIRM_PROJECT="${CONFIRM_PROJECT:-}"
CONFIRM_APPLY="${CONFIRM_APPLY:-}"

RUNTIME_SERVICE_ACCOUNT="${CLOUD_RUN_SERVICE_ACCOUNT_NAME}@${GCP_PROJECT_ID}.iam.gserviceaccount.com"
EXPECTED_CONFIRMATION="APPLY ${GCP_PROJECT_ID}/${GCP_REGION}/${SERVICE_NAME}/${DB_INSTANCE_NAME}"

export CLOUDSDK_CORE_DISABLE_PROMPTS=1

section() {
  printf '\n== %s ==\n' "$1"
}

project_roles_for() {
  gcloud projects get-iam-policy "$GCP_PROJECT_ID" \
    --flatten='bindings[].members' \
    --filter="bindings.members:serviceAccount:${1}" \
    --format='value(bindings.role)' | sort -u
}

print_project_roles() {
  local service_account="$1"
  local roles

  roles="$(project_roles_for "$service_account")"
  if [[ -z "$roles" ]]; then
    echo "  (none)"
    return
  fi

  while IFS= read -r role; do
    printf '  %s\n' "$role"
  done <<< "$roles"
}

print_secret_roles() {
  local service_account="$1"
  local roles

  roles="$(
    gcloud secrets get-iam-policy "$DB_SECRET_NAME" \
      --project "$GCP_PROJECT_ID" \
      --flatten='bindings[].members' \
      --filter="bindings.members:serviceAccount:${service_account}" \
      --format='value(bindings.role)' | sort -u
  )"

  if [[ -z "$roles" ]]; then
    echo "  (none)"
    return
  fi

  while IFS= read -r role; do
    printf '  %s\n' "$role"
  done <<< "$roles"
}

service_account_exists() {
  gcloud iam service-accounts describe "$1" \
    --project "$GCP_PROJECT_ID" \
    --format='none' >/dev/null 2>&1
}

audit_service_account() {
  local label="$1"
  local service_account="$2"

  printf '%s: %s\n' "$label" "$service_account"
  if service_account_exists "$service_account"; then
    gcloud iam service-accounts describe "$service_account" \
      --project "$GCP_PROJECT_ID" \
      --format='yaml(email,displayName,disabled)'
  else
    echo "  Service account does not exist."
  fi

  echo "Project-level roles:"
  print_project_roles "$service_account"
  printf 'Secret-level roles on %s:\n' "$DB_SECRET_NAME"
  print_secret_roles "$service_account"
}

validate_configuration() {
  if [[ "$GCP_PROJECT_ID" != "$TARGET_PROJECT_ID" ]]; then
    echo "Refusing project ${GCP_PROJECT_ID}; TS35 is restricted to ${TARGET_PROJECT_ID}." >&2
    exit 1
  fi

  if [[ ! "$CLOUD_RUN_SERVICE_ACCOUNT_NAME" =~ ^[a-z][a-z0-9-]{4,28}[a-z0-9]$ ]]; then
    echo "Invalid Cloud service account name: ${CLOUD_RUN_SERVICE_ACCOUNT_NAME}" >&2
    exit 1
  fi

  if [[ ! "$CLOUD_SQL_BACKUP_START_TIME" =~ ^([01][0-9]|2[0-3]):[0-5][0-9]$ ]]; then
    echo "CLOUD_SQL_BACKUP_START_TIME must use HH:MM in UTC." >&2
    exit 1
  fi

  if [[ "$APPLY" != "false" && "$APPLY" != "true" ]]; then
    echo "APPLY must be exactly true or false." >&2
    exit 1
  fi
}

verify_project() {
  local actual_project_id
  local actual_project_number

  actual_project_id="$(
    gcloud projects describe "$GCP_PROJECT_ID" --format='value(projectId)'
  )"
  actual_project_number="$(
    gcloud projects describe "$GCP_PROJECT_ID" --format='value(projectNumber)'
  )"

  if [[ "$actual_project_id" != "$TARGET_PROJECT_ID" || "$actual_project_number" != "$TARGET_PROJECT_NUMBER" ]]; then
    echo "Target verification failed; expected ${TARGET_PROJECT_ID} (${TARGET_PROJECT_NUMBER})." >&2
    exit 1
  fi
}

audit_runtime() {
  local current_runtime_service_account

  section "Target project"
  gcloud projects describe "$GCP_PROJECT_ID" \
    --format='yaml(projectId,projectNumber,lifecycleState)'

  section "Cloud SQL security metadata"
  gcloud sql instances describe "$DB_INSTANCE_NAME" \
    --project "$GCP_PROJECT_ID" \
    --format='yaml(name,project,region,databaseVersion,settings.ipConfiguration.authorizedNetworks,settings.ipConfiguration.sslMode,settings.ipConfiguration.requireSsl,settings.deletionProtectionEnabled,settings.backupConfiguration.enabled,settings.backupConfiguration.startTime,settings.backupConfiguration.binaryLogEnabled,settings.backupConfiguration.pointInTimeRecoveryEnabled,settings.backupConfiguration.transactionLogRetentionDays)'

  section "Cloud Run runtime metadata"
  gcloud run services describe "$SERVICE_NAME" \
    --project "$GCP_PROJECT_ID" \
    --region "$GCP_REGION" \
    --format='yaml(metadata.name,status.url,status.latestReadyRevisionName,spec.template.spec.serviceAccountName)'

  current_runtime_service_account="$(
    gcloud run services describe "$SERVICE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --region "$GCP_REGION" \
      --format='value(spec.template.spec.serviceAccountName)'
  )"

  section "Secret metadata"
  echo "Only the secret resource and IAM policy are inspected; secret versions are never accessed."
  gcloud secrets describe "$DB_SECRET_NAME" \
    --project "$GCP_PROJECT_ID" \
    --format='yaml(name)'

  section "Runtime IAM"
  audit_service_account "Current Cloud Run identity" "$current_runtime_service_account"
  if [[ "$current_runtime_service_account" != "$RUNTIME_SERVICE_ACCOUNT" ]]; then
    echo
    audit_service_account "Dedicated runtime identity" "$RUNTIME_SERVICE_ACCOUNT"
  fi
}

ensure_dedicated_account_has_no_broad_roles() {
  local roles

  if ! service_account_exists "$RUNTIME_SERVICE_ACCOUNT"; then
    return
  fi

  roles="$(project_roles_for "$RUNTIME_SERVICE_ACCOUNT")"
  while IFS= read -r role; do
    [[ -z "$role" || "$role" == "roles/cloudsql.client" ]] && continue
    echo "Refusing apply: ${RUNTIME_SERVICE_ACCOUNT} already has unexpected project role ${role}." >&2
    echo "Review and remove broad roles before using it as the runtime identity." >&2
    exit 1
  done <<< "$roles"
}

confirm_apply() {
  local response

  if [[ "$CONFIRM_PROJECT" != "$GCP_PROJECT_ID" ]]; then
    echo "APPLY=true also requires CONFIRM_PROJECT=${GCP_PROJECT_ID}." >&2
    exit 1
  fi

  echo
  echo "Cloud SQL may restart while SSL enforcement and PITR are enabled."
  echo "The allowed target is: ${GCP_PROJECT_ID}/${GCP_REGION}/${SERVICE_NAME}/${DB_INSTANCE_NAME}"

  if [[ -t 0 ]]; then
    read -r -p "Type '${EXPECTED_CONFIRMATION}' to continue: " response
  else
    response="$CONFIRM_APPLY"
  fi

  if [[ "$response" != "$EXPECTED_CONFIRMATION" ]]; then
    echo "Apply confirmation did not match; no changes were made." >&2
    exit 1
  fi
}

apply_runtime_hardening() {
  local authorized_networks
  local backup_enabled
  local binary_log_enabled
  local current_runtime_service_account
  local database_version
  local deletion_protection
  local require_ssl
  local ssl_mode
  local -a sql_patch_args=()

  database_version="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(databaseVersion)'
  )"
  if [[ "$database_version" != MYSQL_* ]]; then
    echo "Refusing apply: ${DB_INSTANCE_NAME} is ${database_version}, not MySQL." >&2
    exit 1
  fi

  ensure_dedicated_account_has_no_broad_roles
  confirm_apply

  section "Apply dedicated runtime IAM"
  if service_account_exists "$RUNTIME_SERVICE_ACCOUNT"; then
    echo "Using existing service account: $RUNTIME_SERVICE_ACCOUNT"
  else
    gcloud iam service-accounts create "$CLOUD_RUN_SERVICE_ACCOUNT_NAME" \
      --project "$GCP_PROJECT_ID" \
      --display-name='ColdTrace Cloud Run runtime' \
      --description='Least-privilege runtime identity for the ColdTrace Cloud Run service' \
      --format='value(email)'
  fi

  gcloud projects add-iam-policy-binding "$GCP_PROJECT_ID" \
    --member="serviceAccount:${RUNTIME_SERVICE_ACCOUNT}" \
    --role='roles/cloudsql.client' \
    --condition=None \
    --quiet \
    --format='none'
  echo "Granted project role roles/cloudsql.client."

  gcloud secrets add-iam-policy-binding "$DB_SECRET_NAME" \
    --project "$GCP_PROJECT_ID" \
    --member="serviceAccount:${RUNTIME_SERVICE_ACCOUNT}" \
    --role='roles/secretmanager.secretAccessor' \
    --condition=None \
    --quiet \
    --format='none'
  echo "Granted roles/secretmanager.secretAccessor on secret ${DB_SECRET_NAME} only."

  ensure_dedicated_account_has_no_broad_roles

  authorized_networks="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --flatten='settings.ipConfiguration.authorizedNetworks[]' \
      --format='value(settings.ipConfiguration.authorizedNetworks.value)'
  )"
  ssl_mode="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(settings.ipConfiguration.sslMode)'
  )"
  require_ssl="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(settings.ipConfiguration.requireSsl)'
  )"
  deletion_protection="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(settings.deletionProtectionEnabled)'
  )"
  backup_enabled="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(settings.backupConfiguration.enabled)'
  )"
  binary_log_enabled="$(
    gcloud sql instances describe "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --format='value(settings.backupConfiguration.binaryLogEnabled)'
  )"

  [[ -n "$authorized_networks" ]] && sql_patch_args+=(--clear-authorized-networks)
  if [[ "$ssl_mode" != "ENCRYPTED_ONLY" || "$require_ssl" != "True" ]]; then
    sql_patch_args+=(--ssl-mode=ENCRYPTED_ONLY --require-ssl)
  fi
  [[ "$deletion_protection" != "True" ]] && sql_patch_args+=(--deletion-protection)
  [[ "$backup_enabled" != "True" ]] && sql_patch_args+=(--backup-start-time="$CLOUD_SQL_BACKUP_START_TIME")
  [[ "$binary_log_enabled" != "True" ]] && sql_patch_args+=(--enable-bin-log)

  section "Apply Cloud SQL hardening"
  if (( ${#sql_patch_args[@]} == 0 )); then
    echo "Cloud SQL hardening settings are already applied."
  else
    gcloud sql instances patch "$DB_INSTANCE_NAME" \
      --project "$GCP_PROJECT_ID" \
      "${sql_patch_args[@]}" \
      --quiet \
      --format='none'
    echo "Cloud SQL hardening patch completed."
  fi

  current_runtime_service_account="$(
    gcloud run services describe "$SERVICE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --region "$GCP_REGION" \
      --format='value(spec.template.spec.serviceAccountName)'
  )"

  section "Apply Cloud Run runtime identity"
  if [[ "$current_runtime_service_account" == "$RUNTIME_SERVICE_ACCOUNT" ]]; then
    echo "Cloud Run already uses ${RUNTIME_SERVICE_ACCOUNT}."
  else
    gcloud run services update "$SERVICE_NAME" \
      --project "$GCP_PROJECT_ID" \
      --region "$GCP_REGION" \
      --service-account "$RUNTIME_SERVICE_ACCOUNT" \
      --quiet \
      --format='value(status.latestCreatedRevisionName)'
  fi

  section "Post-apply audit"
  audit_runtime
  echo
  echo "Apply complete. Run the documented Cloud Run smoke checks before removing access from the previous identity."
}

validate_configuration
verify_project
audit_runtime

if [[ "$APPLY" == "false" ]]; then
  echo
  echo "Audit complete. APPLY=false; no cloud resources were changed."
  exit 0
fi

apply_runtime_hardening
