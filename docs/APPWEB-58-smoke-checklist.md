# TS07 Smoke Checklist

Ticket: T-51 / TS07 - Sensor Readings API

## Prerequisites

- MySQL is running locally.
- The configured user/password is `root` / `root`.
- The API starts successfully and Swagger is available at `/swagger/index.html`.
- At least one organization, one location, one gateway, one asset, and one IoT device exist in the same organization.

## Manual Checks

1. Create an organization through `POST /api/v1/organization-sign-ups`.
2. Create a location under that organization through `POST /api/v1/organizations/{organizationId}/locations`.
3. Create a gateway under that organization through `POST /api/v1/organizations/{organizationId}/gateways`.
4. Create an asset under the same organization and location through `POST /api/v1/organizations/{organizationId}/assets`.
5. Create an IoT device through `POST /api/v1/organizations/{organizationId}/iot-devices`.
6. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings` returns `200` and an empty array for a new organization.
7. Create a sensor reading through `POST /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}/sensor-readings`.
8. Confirm the response is `201` and includes `id`, `organizationId`, `iotDeviceId`, `metric`, `value`, `unit`, and `recordedAt`.
9. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings` returns the created reading.
10. Confirm `GET /api/v1/organizations/{organizationId}/sensor-readings/{sensorReadingId}` returns the created reading.
11. Confirm `GET /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}/sensor-readings` returns the created reading.
12. Confirm creating a reading through another organization scope returns `404`.
13. Confirm requesting the reading through another organization scope returns `404`.

## Endpoint Coverage

- `GET /api/v1/organizations/{organizationId}/sensor-readings`
- `GET /api/v1/organizations/{organizationId}/sensor-readings/{sensorReadingId}`
- `GET /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}/sensor-readings`
- `POST /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}/sensor-readings`

## Contract Notes

- Sensor readings are append-only in this ticket.
- `recordedAt` is optional on create; when omitted, the backend assigns the current timestamp.
- The request payload is `metric`, `value`, `unit`, and optional `recordedAt`.
- The response payload includes `id`, `organizationId`, `iotDeviceId`, `metric`, `value`, `unit`, and `recordedAt`.
- There are no update or delete endpoints in this ticket.
