# APPWEB-70 Smoke Checklist

Ticket: APPWEB-70 / T-61 / TS13 - Gateways API

## Preconditions

- MySQL is running locally.
- The configured user/password is `root` / `root`.
- The API starts successfully and Swagger is available at `/swagger/index.html`.
- At least one organization and one organization-scoped location exist.

## Manual Checks

1. Create two organizations through `POST /api/v1/organization-sign-ups`.
2. Create one location under the first organization through `POST /api/v1/organizations/{organizationId}/locations`.
3. Create one location under the second organization.
4. Confirm `GET /api/v1/organizations/{organizationId}/gateways` returns `200` and an empty array for a new organization.
5. Create a gateway through `POST /api/v1/organizations/{organizationId}/gateways` using the first organization's location.
6. Confirm the response is `201` and includes `id`, `organizationId`, `locationId`, `uuid`, `name`, `network`, and `status`.
7. Confirm `GET /api/v1/organizations/{organizationId}/gateways/{gatewayId}` returns the created gateway.
8. Update the gateway through `PUT /api/v1/organizations/{organizationId}/gateways/{gatewayId}`.
9. Confirm creating another gateway with the same `uuid` in the same organization returns `409`.
10. Confirm creating a gateway with a `locationId` from another organization returns `404`.
11. Confirm requesting the gateway through another organization scope returns `404`.

## Endpoint Coverage

- `GET /api/v1/organizations/{organizationId}/gateways`
- `GET /api/v1/organizations/{organizationId}/gateways/{gatewayId}`
- `POST /api/v1/organizations/{organizationId}/gateways`
- `PUT /api/v1/organizations/{organizationId}/gateways/{gatewayId}`
