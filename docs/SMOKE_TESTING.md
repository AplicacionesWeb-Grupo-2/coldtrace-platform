# Smoke Testing Guide

This guide summarizes the current manual verification flow for the ColdTrace Platform backend.

## Latest Local Verification

Last verified state:

- Branch: `develop`
- Build command: `/Users/mauriciopajes/.dotnet/dotnet build coldtrace-platform/coldtrace-platform.csproj`
- Result: `0` warnings, `0` errors
- API smoke flow: `53` HTTP requests, `0` failures
- Database: 18 domain tables plus `__EFMigrationsHistory`

The smoke flow covered the main frontend-facing workflows:

- Swagger availability
- Organization sign-up
- Roles and users
- Locations
- Gateways
- Assets
- Asset settings
- IoT devices
- Sensor readings
- Incident lifecycle
- Notifications
- Subscription plan catalog
- Maintenance schedules
- Technical service requests
- Reports

## Preconditions

- MySQL is running locally.
- The API can connect to `coldtrace_platform`.
- The default local credentials are `root` / `root`, unless overridden through `ConnectionStrings__DefaultConnection`.
- The API starts successfully and Swagger is available at `/swagger/index.html`.
- EF Core migrations are applied on startup.

Run locally:

```bash
ASPNETCORE_ENVIRONMENT=Development \
/Users/mauriciopajes/.dotnet/dotnet run \
  --project coldtrace-platform/coldtrace-platform.csproj \
  --urls http://localhost:5271
```

## Recommended Smoke Flow

1. Open `/swagger/index.html`.
2. Confirm `/swagger/v1/swagger.json` returns `200`.
3. List subscription plans with `GET /api/v1/subscription-plans` and expect `base`, `operations`, and `compliance-ai`.
4. Create an organization and first user with `POST /api/v1/organization-sign-ups`.
5. Get the new organization's subscription with `GET /api/v1/organizations/{organizationId}/subscription` and expect `plan.code` `base`, `status` `FREE`, `provider` `NONE`, usage counters, and `LIMIT`/`FEATURE` entitlements.
6. With Stripe not configured, request `POST /api/v1/organizations/{organizationId}/billing/checkout-sessions` for `operations` and expect `503`.
7. With `STRIPE_SECRET_KEY`, `STRIPE_OPERATIONS_PRICE_ID`, `BILLING_CHECKOUT_SUCCESS_URL`, and `BILLING_CHECKOUT_CANCEL_URL` configured, request the same checkout endpoint and expect `200` with `provider`, `sessionId`, `checkoutUrl`, and `targetPlanCode`.
8. With a paid Stripe customer id stored on the organization subscription, request `POST /api/v1/organizations/{organizationId}/billing/portal-sessions` and expect `200` with `provider`, `sessionId`, `portalUrl`, and `organizationId`; without a provider customer id, expect `409`.
9. With `STRIPE_WEBHOOK_SECRET` missing, request `POST /api/v1/billing/stripe/webhooks` and expect `503`; with the secret configured, signed Stripe events should return `200` with `provider`, `eventId`, `eventType`, `processingStatus`, `duplicate`, `organizationId`, `planCode`, and `subscriptionStatus`.
10. While the organization is still on `base`, exceed a plan-limited operation such as creating a second location or fourth user, and expect `409` with `entitlementKey`, `planCode`, `subscriptionStatus`, `limit`, `used`, `remaining`, `lockedReason`, and `requiredPlanCode`.
11. List roles with `GET /api/v1/roles`.
12. Create an organization-scoped user with `POST /api/v1/organizations/{organizationId}/users`.
13. Assign or replace the user's role with `PATCH /api/v1/organizations/{organizationId}/users/{userId}/role`.
14. Create at least one location with `POST /api/v1/organizations/{organizationId}/locations`.
15. Create at least one gateway with `POST /api/v1/organizations/{organizationId}/gateways`.
16. Create at least one asset with `POST /api/v1/organizations/{organizationId}/assets`.
17. Save default asset settings with `PUT /api/v1/organizations/{organizationId}/asset-settings/default`.
18. Save asset-specific settings with `PUT /api/v1/organizations/{organizationId}/assets/{assetId}/settings`.
19. Create an assigned IoT device with `POST /api/v1/organizations/{organizationId}/iot-devices`.
20. Create telemetry with `POST /api/v1/organizations/{organizationId}/sensor-readings`.
21. Query telemetry with `GET /api/v1/organizations/{organizationId}/sensor-readings?assetId={assetId}&iotDeviceId={iotDeviceId}`.
22. Generate demo telemetry with `POST /api/v1/organizations/{organizationId}/sensor-readings/demo-generations`.
22. Register an incident with `POST /api/v1/organizations/{organizationId}/incidents`.
23. Acknowledge the incident with `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/acknowledgements`.
24. Escalate it with `PATCH /api/v1/organizations/{organizationId}/incidents/{incidentId}/escalation`.
25. Register corrective action with `PATCH /api/v1/organizations/{organizationId}/incidents/{incidentId}/corrective-action`.
26. With AI disabled or unconfigured, request `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans` and expect `503` without creating a plan.
27. List plan history with `GET /api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans` and expect `200` with an array, even when no plans exist.
28. When AI is enabled and a pending plan exists, approve it with `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans/{planId}/approvals` and expect `200` with `status: "approved"`, `approvedBy`, `finalCorrectiveAction`, and `finalResolutionNotes`.
29. For a separate pending plan, reject it with `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans/{planId}/rejections` and expect `200` with `status: "rejected"`, `rejectedBy`, and `rejectionReason` while the incident remains open or acknowledged.
30. Resolve it with `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/resolutions` when not using the AI approval path.
31. Request `POST /api/v1/organizations/{organizationId}/incidents/{incidentId}/ai-resolution-plans` again and expect `409` because resolved incidents cannot receive new AI plans.
32. Check incident notifications with `GET /api/v1/organizations/{organizationId}/incidents/{incidentId}/notifications`.
33. Create a maintenance schedule with `POST /api/v1/organizations/{organizationId}/maintenance-schedules`.
34. Update the maintenance schedule status with `PATCH /api/v1/organizations/{organizationId}/maintenance-schedules/{maintenanceScheduleId}`.
35. Create a technical service request with `POST /api/v1/organizations/{organizationId}/technical-service-requests`.
36. Update the technical service request status with `PATCH /api/v1/organizations/{organizationId}/technical-service-requests/{technicalServiceRequestId}`.
37. Generate an operational report with `POST /api/v1/organizations/{organizationId}/reports`.
38. Confirm the report can be listed and read with `GET /api/v1/organizations/{organizationId}/reports` and `GET /api/v1/organizations/{organizationId}/reports/{reportId}`.
39. With AI disabled or unconfigured, request `POST /api/v1/organizations/{organizationId}/reports/{reportId}/ai-summary` and expect `503`.
40. When AI is enabled, request the same report AI summary endpoint and expect `200` with `sourceReport`, `executiveSummary`, `findings`, `evidenceGaps`, `recommendedActions`, `uncertaintyNotes`, `modelProvider`, and `modelName`.

## Expected Status Codes

| Operation | Expected status |
| --- | --- |
| Create organization, sign-up, user, location, gateway, asset, device, reading, incident, schedule, request, report | `201` |
| List or read resources | `200` |
| Update resources or lifecycle state | `200` |
| Duplicate unique resources | `409` |
| Cross-organization resource access | `404` |
| Invalid payloads or unsupported lifecycle inputs | `400` |

## Data Notes

- Test data should remain internally consistent: organization, location, gateway, asset, IoT device, readings, incidents, maintenance, and reports must belong to the same organization unless the check is intentionally validating a `404`.
- For IoT devices assigned to an asset, the gateway and asset must belong to the same organization and compatible location.
- Sensor readings require an eligible assigned IoT device and evaluate temperature/humidity against the effective asset settings.
- Generated demo readings are limited by the request `count` and only use eligible devices.

## Ticket-Level Checklists

Detailed manual checks are also available:

- [APPWEB-49 IoT Devices](APPWEB-49-smoke-checklist.md)
- [APPWEB-51 API Foundation](APPWEB-51-smoke-checklist.md)
- [APPWEB-52 Organization Sign-Up](APPWEB-52-smoke-checklist.md)
- [APPWEB-54 Users API](APPWEB-54-smoke-checklist.md)
- [APPWEB-58 Sensor Readings](APPWEB-58-smoke-checklist.md)
- [APPWEB-70 Gateways API](APPWEB-70-smoke-checklist.md)
- [APPWEB-73 Organizations API Base](APPWEB-73-smoke-checklist.md)
- [APPWEB-74 Locations API](APPWEB-74-smoke-checklist.md)
