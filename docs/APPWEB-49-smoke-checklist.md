# APPWEB-49 Smoke Checklist

Ticket: APPWEB-49 / T-49 / TS14 - IoT Devices API

## Preconditions

- MySQL is running locally.
- The configured user/password is `root` / `root`.
- The API starts successfully and Swagger is available at `/swagger/index.html`.
- At least one organization, one location, one gateway, and one asset exist in the same organization.
- `assetId` is optional on create and update, but when it is present the asset must belong to the same organization and be compatible with the selected gateway.

## Manual Checks

1. Create an organization through `POST /api/v1/organization-sign-ups`.
2. Create a location under that organization through `POST /api/v1/organizations/{organizationId}/locations`.
3. Create a gateway under that organization through `POST /api/v1/organizations/{organizationId}/gateways`.
4. Create an asset under the same organization and location through `POST /api/v1/organizations/{organizationId}/assets`.
5. Confirm `GET /api/v1/organizations/{organizationId}/iot-devices` returns `200` and an empty array for a new organization.
6. Create an IoT device through `POST /api/v1/organizations/{organizationId}/iot-devices` using the created gateway and asset.
7. Confirm the response is `201` and includes `id`, `organizationId`, `gatewayId`, `assetId`, `uuid`, `deviceType`, `model`, `measurementType`, `measurementParameters`, `readingFrequencySeconds`, `status`, `calibrationStatus`, `lastCalibrationDate`, and `nextCalibrationDate`.
8. Confirm `GET /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}` returns the created device.
9. Update the device through `PUT /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}`.
10. Confirm creating another device with the same `uuid` in the same organization returns `409`.
11. Confirm creating a device with a gateway from another organization returns `404`.
12. Confirm creating a device with an asset from another organization returns `404`.
13. Confirm creating a device with an asset from a different location than the gateway returns `409`.
14. Confirm requesting the device through another organization scope returns `404`.

## Contract Notes

- The collection endpoint is `GET /api/v1/organizations/{organizationId}/iot-devices`.
- The item endpoint is `GET /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}`.
- Create and update accept `gatewayId`, optional `assetId`, `uuid`, `deviceType`, `model`, `measurementType`, optional `measurementParameters`, `readingFrequencySeconds`, `status`, `calibrationStatus`, `lastCalibrationDate`, and `nextCalibrationDate`.
- `POST` returns `201` with the created resource body.
- `PUT` returns `200` with the updated resource body.
- `uuid` is unique per organization.
- An asset assigned to an IoT device must belong to the same organization and be compatible with the selected gateway.

## Endpoint Coverage

- `GET /api/v1/organizations/{organizationId}/iot-devices`
- `GET /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}`
- `POST /api/v1/organizations/{organizationId}/iot-devices`
- `PUT /api/v1/organizations/{organizationId}/iot-devices/{iotDeviceId}`
