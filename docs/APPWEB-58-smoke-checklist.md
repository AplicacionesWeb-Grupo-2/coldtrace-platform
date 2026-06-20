# APPWEB-58 Smoke Checklist

Ticket: T-51 / TS07 - Sensor Readings API

## Prerequisites

- MySQL is running locally.
- The configured user/password is `root` / `root`, unless overridden locally.
- The API starts successfully and Swagger is available at `/swagger/index.html`.
- At least one organization, one location, one gateway, one asset, one effective asset settings record, and one assigned IoT device exist in the same organization.
- The IoT device is assigned to the asset and has a valid gateway.

## Manual Checks

1. Create an organization through `POST /api/v1/organization-sign-ups`.
2. Create a location under that organization through `POST /api/v1/organizations/{organizationId}/locations`.
3. Create a gateway under that organization through `POST /api/v1/organizations/{organizationId}/gateways`.
4. Create an asset under the same organization and location through `POST /api/v1/organizations/{organizationId}/assets`.
5. Save default or asset-specific settings through `PUT /api/v1/organizations/{organizationId}/asset-settings/default` or `PUT /api/v1/organizations/{organizationId}/assets/{assetId}/settings`.
6. Create an assigned IoT device through `POST /api/v1/organizations/{organizationId}/iot-devices`.
7. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings` returns `200` and an empty array for a new organization.
8. Create a sensor reading through `POST /api/v1/organizations/{organizationId}/sensor-readings`.
9. Confirm the response is `201` and includes `id`, `organizationId`, `assetId`, `iotDeviceId`, `gatewayId`, `locationId`, `temperature`, `humidity`, `outOfRange`, `isOutOfRange`, `recordedAt`, `motionDetected`, `imageCaptured`, `batteryLevel`, and `signalStrength`.
10. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings` returns the created reading.
11. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings/{sensorReadingId}` returns the created reading.
12. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings?assetId={assetId}&iotDeviceId={iotDeviceId}` returns readings matching the filters.
13. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings?from={from}&to={to}` returns readings inside the date range.
14. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings?from={later}&to={earlier}` returns `400`.
15. Confirm creating a reading through another organization scope returns `404`.
16. Confirm creating a reading for an unassigned or incompatible device returns `400` or `404`, depending on the invalid input.
17. Generate demo readings through `POST /api/v1/organizations/{organizationId}/sensor-readings/demo-generations` with optional `assetId` and `count`.
18. Confirm demo generation returns `201` and an array with up to the requested `count`.

## Endpoint Coverage

- `GET /api/v1/organizations/{organizationId}/sensor-readings`
- `GET /api/v1/organizations/{organizationId}/sensor-readings/{sensorReadingId}`
- `POST /api/v1/organizations/{organizationId}/sensor-readings`
- `POST /api/v1/organizations/{organizationId}/sensor-readings/demo-generations`

## Contract Notes

- Sensor readings are append-only.
- `recordedAt` is optional on create; when omitted, the backend assigns the current timestamp.
- Create payload: `assetId`, `iotDeviceId`, optional `temperature`, optional `humidity`, optional `recordedAt`, optional `motionDetected`, optional `imageCaptured`, optional `batteryLevel`, and optional `signalStrength`.
- `batteryLevel` and `signalStrength` must be between `0` and `100`.
- Readings are evaluated against effective asset settings and return both `outOfRange` and `isOutOfRange` for client compatibility.
- The list endpoint supports optional `assetId`, `iotDeviceId`, `from`, and `to` query filters.
- There are no update or delete endpoints in this ticket.
