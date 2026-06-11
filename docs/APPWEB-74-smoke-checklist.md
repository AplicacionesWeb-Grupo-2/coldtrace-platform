# APPWEB-74 Smoke Checklist

Ticket: APPWEB-74 / T-65 / TS17 - Locations API

## Preconditions

- MySQL is running locally.
- The configured user/password is `root` / `root`.
- The API starts successfully and Swagger is available at `/swagger/index.html`.

## Manual Checks

1. Create an organization through `POST /api/v1/organization-sign-ups`.
2. Confirm `GET /api/v1/organizations/{organizationId}/locations` returns `200` and an empty array for a new organization.
3. Create a location through `POST /api/v1/organizations/{organizationId}/locations`.
4. Confirm the response is `201` and includes `id`, `organizationId`, `name`, `type`, `address`, `description`, and `status`.
5. Confirm `GET /api/v1/organizations/{organizationId}/locations/{locationId}` returns the created location.
6. Update the location through `PUT /api/v1/organizations/{organizationId}/locations/{locationId}`.
7. Confirm creating a second location with the same `name` in the same organization returns `409`.
8. Confirm requesting the location through another organization scope returns `404`.
9. Confirm creating a location for a missing organization returns `404`.

## Endpoint Coverage

- `GET /api/v1/organizations/{organizationId}/locations`
- `GET /api/v1/organizations/{organizationId}/locations/{locationId}`
- `POST /api/v1/organizations/{organizationId}/locations`
- `PUT /api/v1/organizations/{organizationId}/locations/{locationId}`
