# ColdTrace Platform API Reference

Base URL for local development:

```text
http://localhost:5271
```

Swagger UI:

```text
/swagger/index.html
```

All request and response bodies use JSON. Most operational endpoints are scoped by organization:

```text
/api/v1/organizations/{organizationId}
```

## Identity Access

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations` | List organizations. |
| `POST` | `/api/v1/organizations` | Create an organization. |
| `POST` | `/api/v1/organization-sign-ups` | Create an organization and its first user. |
| `GET` | `/api/v1/roles` | List seeded roles and permissions. |
| `GET` | `/api/v1/organizations/{organizationId}/users` | List users for one organization. |
| `POST` | `/api/v1/organizations/{organizationId}/users` | Create an organization user. |
| `PATCH` | `/api/v1/organizations/{organizationId}/users/{userId}/role` | Assign or replace a user's role. |

Important request fields:

- `POST /organization-sign-ups`: `legalName`, `commercialName`, `taxId`, `contactEmail`, `firstName`, `lastName`, `email`.
- `POST /organizations/{organizationId}/users`: `firstName`, `lastName`, `email`, `roleId`.
- `PATCH /users/{userId}/role`: `roleId`.

## Asset Management

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations/{organizationId}/locations` | List locations. |
| `GET` | `/api/v1/organizations/{organizationId}/locations/{locationId}` | Get one location. |
| `POST` | `/api/v1/organizations/{organizationId}/locations` | Create a location. |
| `PUT` | `/api/v1/organizations/{organizationId}/locations/{locationId}` | Update a location. |
| `GET` | `/api/v1/organizations/{organizationId}/gateways` | List gateways. |
| `GET` | `/api/v1/organizations/{organizationId}/gateways/{gatewayId}` | Get one gateway. |
| `POST` | `/api/v1/organizations/{organizationId}/gateways` | Create a gateway. |
| `PUT` | `/api/v1/organizations/{organizationId}/gateways/{gatewayId}` | Update a gateway. |
| `GET` | `/api/v1/organizations/{organizationId}/assets` | List assets. |
| `GET` | `/api/v1/organizations/{organizationId}/assets/{assetId}` | Get one asset. |
| `POST` | `/api/v1/organizations/{organizationId}/assets` | Create an asset. |
| `PUT` | `/api/v1/organizations/{organizationId}/assets/{assetId}` | Update an asset. |
| `GET` | `/api/v1/organizations/{organizationId}/asset-settings` | List default and asset-specific settings. |
| `GET` | `/api/v1/organizations/{organizationId}/assets/{assetId}/settings` | Get effective settings for one asset. |
| `PUT` | `/api/v1/organizations/{organizationId}/asset-settings/default` | Create or update organization default settings. |
| `PUT` | `/api/v1/organizations/{organizationId}/assets/{assetId}/settings` | Create or update settings for one asset. |
| `GET` | `/api/v1/organizations/{organizationId}/iot-devices` | List IoT devices. |
| `GET` | `/api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}` | Get one IoT device. |
| `POST` | `/api/v1/organizations/{organizationId}/iot-devices` | Create an IoT device. |
| `PUT` | `/api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}` | Update an IoT device. |

Important request fields:

- Locations: `name`, `type`, `address`, `description`, `status`.
- Gateways: `locationId`, `uuid`, `name`, `network`, `status`.
- Assets: `locationId`, `uuid`, `type`, `name`, `capacity`, `description`, `status`.
- Asset settings: `uuid`, `assetTypes`, `iotDeviceTypes`, `minimumTemperature`, `maximumTemperature`, `minimumHumidity`, `maximumHumidity`, `calibrationFrequencyDays`, `temperatureUnit`, `humidityUnit`, `weightUnit`, `readingFrequencySeconds`, `alertThresholdMinutes`.
- IoT devices: `gatewayId`, `uuid`, `deviceType`, `model`, `measurementType`, `measurementParameters`, `readingFrequencySeconds`, `assetId`, `status`, `calibrationStatus`, `lastCalibrationDate`, `nextCalibrationDate`.

Validation rules enforced by the API include organization ownership, unique UUIDs per organization for gateways/assets/devices, and gateway/asset location compatibility for assigned IoT devices.

## Monitoring

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations/{organizationId}/sensor-readings` | List readings with optional query filters. |
| `GET` | `/api/v1/organizations/{organizationId}/sensor-readings/{sensorReadingId}` | Get one reading. |
| `POST` | `/api/v1/organizations/{organizationId}/sensor-readings` | Persist telemetry and evaluate thresholds. |
| `POST` | `/api/v1/organizations/{organizationId}/sensor-readings/demo-generations` | Generate demo readings for eligible devices. |

Supported query filters for `GET /sensor-readings`:

```text
assetId
iotDeviceId
from
to
```

Create reading request fields:

```text
assetId
iotDeviceId
temperature
humidity
recordedAt
motionDetected
imageCaptured
batteryLevel
signalStrength
```

The response includes `gatewayId`, `locationId`, `outOfRange`, and `isOutOfRange` so frontend views can avoid recalculating ownership and compliance state.

## Alerts

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations/{organizationId}/incidents` | List incidents. |
| `GET` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}` | Get one incident. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents` | Register an incident. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/acknowledgements` | Acknowledge an incident. |
| `PATCH` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/escalation` | Register escalation details. |
| `PATCH` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/corrective-action` | Register corrective action details. |
| `GET` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans` | List generated, approved, and rejected AI resolution plans for one incident. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans` | Generate and persist a pending AI resolution plan. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans/{planId}/approvals` | Approve a pending AI resolution plan and resolve the incident. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans/{planId}/rejections` | Reject a pending AI resolution plan without changing incident state. |
| `POST` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/resolutions` | Resolve an incident. |
| `GET` | `/api/v1/organizations/{organizationId}/incidents/{incidentId}/notifications` | List notifications for one incident. |
| `GET` | `/api/v1/organizations/{organizationId}/notifications` | List organization notifications. |

Incident lifecycle endpoints return `409 Conflict` when a transition is not allowed.
AI resolution plan generation also returns `409 Conflict` for resolved incidents or incidents with incomplete referenced context, `502 Bad Gateway` for invalid structured provider output, `503 Service Unavailable` when AI is disabled/unconfigured/unavailable, and `504 Gateway Timeout` when the provider times out.
AI resolution plan history returns the same resource shape as generation/approval/rejection, ordered from newest to oldest and scoped by organization plus incident.
AI resolution plan approval returns `404 Not Found` when the organization, incident, or plan is missing, and `409 Conflict` when the plan was already approved/rejected or the incident can no longer be resolved.
AI resolution plan rejection returns `404 Not Found` when the organization, incident, or plan is missing, and `409 Conflict` when the plan was already approved or rejected.

Important request fields:

- Create incident: `assetId`, `deviceId`, `readingId`, `assetName`, `deviceName`, `type`, `severity`, `value`.
- Acknowledge: `acknowledgedBy`.
- Escalation: `escalatedBy`, `escalationReason`.
- Corrective action: `correctiveAction`, `registeredBy`.
- Resolution: `resolvedBy`, `resolutionNotes`.
- AI plan approval: `approvedBy`, `finalCorrectiveAction`, `finalResolutionNotes`.
- AI plan rejection: `rejectedBy`, `rejectionReason`.

AI resolution plan generation has no request body. The response mirrors the Spring Boot contract:

```text
id
organizationId
incidentId
status
summary
probableCause
recommendedSteps[{ sequence, action, rationale, expectedOutcome }]
correctiveActionDraft
resolutionNotesDraft
escalationRecommended
escalationUrgency
escalationReason
requiredEvidence
uncertaintyNotes
modelProvider
modelName
providerMetadata
generatedAt
approvedAt
approvedBy
rejectedAt
rejectedBy
rejectionReason
finalCorrectiveAction
finalResolutionNotes
```

AI resolution plan approval accepts:

```json
{
  "approvedBy": "operations.manager@coldtrace.test",
  "finalCorrectiveAction": "Moved inventory to backup freezer and recalibrated the affected sensor",
  "finalResolutionNotes": "Temperature returned to safe range after transfer and recalibration."
}
```

The approval response uses the same `AiResolutionPlanResource` shape with `status` set to `approved`, approval metadata populated, and the final corrective action and resolution notes persisted. The incident is resolved through backend lifecycle rules during the same command.

AI resolution plan rejection accepts:

```json
{
  "rejectedBy": "operations.manager@coldtrace.test",
  "rejectionReason": "Plan requires on-site compressor inspection before closure."
}
```

The rejection response uses the same `AiResolutionPlanResource` shape with `status` set to `rejected`, rejection metadata populated, and the incident lifecycle unchanged.

## Billing

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/subscription-plans` | List visible subscription plans and pricing catalog. |
| `GET` | `/api/v1/organizations/{organizationId}/subscription` | Get an organization's active subscription, usage and entitlements. |
| `POST` | `/api/v1/organizations/{organizationId}/billing/checkout-sessions` | Create a Stripe Checkout session for a paid plan upgrade. |
| `POST` | `/api/v1/organizations/{organizationId}/billing/portal-sessions` | Create a Stripe Customer Portal session for billing management. |
| `POST` | `/api/v1/organizations/{organizationId}/billing/customer-portal-sessions` | Alias for creating a Stripe Customer Portal session. |
| `POST` | `/api/v1/billing/stripe/webhooks` | Process a signed Stripe billing webhook and synchronize local subscription state. |

The subscription plan catalog response mirrors the Spring Boot contract:

```text
id
code
displayName
description
monthlyPriceCents
currency
stripePriceId
recommended
recommendedLabel
visible
usageLimits { maxLocations, maxAssets, maxIotDevices, maxUsers, historyRetentionDays }
featureFlags { allowsExports, allowsMaintenance, allowsAiGuidance, allowsAiReportSummary }
includedFeatures
```

The organization subscription response mirrors the Spring Boot contract:

```text
id
organizationId
status
provider
providerCustomerId
providerSubscriptionId
currentPeriodStart
currentPeriodEnd
cancelAtPeriodEnd
metadata
plan { ...subscription plan resource }
usage { locations, assets, iotDevices, users }
entitlements [{ key, category, enabled, limit, used, remaining, lockedReason }]
```

Restricted writes and paid AI operations use those backend-computed entitlements before persistence. When the current plan does not allow the operation, the API returns `409 Conflict` with `ProblemDetails` and the Spring Boot-compatible extension fields:

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

Protected operations currently include creating locations, assets, IoT devices, users, reports, maintenance schedules, technical service requests, AI incident resolution plans, and AI report summaries.

Create a checkout session with:

```json
{
  "targetPlanCode": "operations"
}
```

The checkout response returns only safe redirect metadata:

```json
{
  "provider": "STRIPE",
  "sessionId": "cs_test_...",
  "checkoutUrl": "https://checkout.stripe.com/c/pay/cs_test_...",
  "targetPlanCode": "operations"
}
```

The customer portal session response returns only safe redirect metadata and mirrors the Spring Boot contract:

```json
{
  "provider": "STRIPE",
  "sessionId": "bps_test_...",
  "portalUrl": "https://billing.stripe.com/p/session/test_...",
  "organizationId": 1
}
```

The Stripe webhook endpoint requires the `Stripe-Signature` header and returns the processing outcome:

```json
{
  "provider": "STRIPE",
  "eventId": "evt_...",
  "eventType": "checkout.session.completed",
  "processingStatus": "PROCESSED",
  "duplicate": false,
  "organizationId": 1,
  "planCode": "operations",
  "subscriptionStatus": "ACTIVE"
}
```

The public catalog currently returns `base`, `operations`, and `compliance-ai` using backend-owned values. Stripe price identifiers are read from `STRIPE_OPERATIONS_PRICE_ID` and `STRIPE_COMPLIANCE_AI_PRICE_ID` when configured.

## Maintenance Management

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations/{organizationId}/maintenance-schedules` | List maintenance schedules. |
| `GET` | `/api/v1/organizations/{organizationId}/maintenance-schedules/{maintenanceScheduleId}` | Get one maintenance schedule. |
| `POST` | `/api/v1/organizations/{organizationId}/maintenance-schedules` | Create a maintenance schedule. |
| `PATCH` | `/api/v1/organizations/{organizationId}/maintenance-schedules/{maintenanceScheduleId}` | Update schedule lifecycle status. |
| `GET` | `/api/v1/organizations/{organizationId}/technical-service-requests` | List technical service requests. |
| `GET` | `/api/v1/organizations/{organizationId}/technical-service-requests/{technicalServiceRequestId}` | Get one technical service request. |
| `POST` | `/api/v1/organizations/{organizationId}/technical-service-requests` | Create a technical service request. |
| `PATCH` | `/api/v1/organizations/{organizationId}/technical-service-requests/{technicalServiceRequestId}` | Update request lifecycle status. |

Important request fields:

- Maintenance schedule: `assetId`, `scheduledDate`, `frequencyDays`, `responsibleUserId`, `observations`, `status`.
- Maintenance status update: `status`.
- Technical service request: `assetId`, `incidentId`, `issueDescription`, `priority`, `requestedBy`.
- Technical service status update: `status`, `closureSummary`, `evidence`, `closedBy`.

## Reports

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/organizations/{organizationId}/reports` | List generated reports. |
| `POST` | `/api/v1/organizations/{organizationId}/reports` | Generate an operational report. |
| `GET` | `/api/v1/organizations/{organizationId}/reports/{reportId}` | Get one generated report. |
| `POST` | `/api/v1/organizations/{organizationId}/reports/{reportId}/ai-summary` | Generate an advisory AI compliance summary for one report. |

Report generation request fields:

```text
type
title
periodStart
periodEnd
```

The generated report includes asset, reading, incident, average temperature, average humidity, and compliance summary fields.

Report AI summary generation has no request body. The response mirrors the Spring Boot contract:

```text
organizationId
reportId
reportUuid
reportType
reportTitle
summaryGeneratedAt
sourceReport
executiveSummary
findings[{ area, status, evidence, recommendation }]
evidenceGaps
recommendedActions
uncertaintyNotes
modelProvider
modelName
```

Report AI summary generation returns `404 Not Found` when the organization or report is missing, `502 Bad Gateway` for invalid structured provider output, `503 Service Unavailable` when AI is disabled/unconfigured/unavailable, and `504 Gateway Timeout` when the provider times out.

## AI Assistance

| Method | Path | Description |
| --- | --- | --- |
| `GET` | `/api/v1/ai-assistance/provider-status` | Get non-secret AI assistance provider configuration and structured output contract status. |

The AI status response includes:

```text
provider
model
enabled
configured
hasEndpoint
hasApiKey
hasChatClient
timeoutSeconds
structuredOutputContracts
```

Supported provider values are `disabled`, `ollama`, and `openai`.

The structured output contracts prepared by this context are consumed by product
endpoints owned by their respective bounded contexts. Incident AI resolution
plans are exposed from Alerts.

## Error Responses

Expected validation and business-rule responses:

| Status | Meaning |
| --- | --- |
| `400` | Invalid request payload or unsupported input. |
| `404` | Organization or scoped resource was not found. |
| `409` | Duplicate resource or invalid lifecycle transition. |
| `502` | Upstream AI provider returned invalid structured output. |
| `503` | AI provider is disabled, unavailable, or not configured. |
| `504` | AI provider exceeded the configured timeout. |
| `500` | Unexpected server error returned as RFC 7807 `ProblemDetails`. |

Non-validation failures use RFC 7807 `ProblemDetails` with `status`, localized
`title` and `detail`, request-path `instance`, and a stable `code` extension.
For example, a missing organization uses `code: "ORGANIZATION_NOT_FOUND"`.
Request/model validation uses `ValidationProblemDetails` with the same common
fields plus an `errors` object and `code: "VALIDATION_ERROR"`. Error payloads
do not expose exception messages or stack traces.

Send `Accept-Language: es` to receive the existing Spanish shared-resource
messages. Plan-limit conflicts retain their entitlement extension fields, and
Stripe webhook failures retain their existing status behavior while using the
same ProblemDetails contract.

JWT issuance and authenticated-by-default route enforcement are delivered by the dedicated TS02 and T58 security stories; this error contract also applies to their `401` and `403` responses after integration.
