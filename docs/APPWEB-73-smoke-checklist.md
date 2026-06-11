# APPWEB-73 Smoke Checklist

Ticket: T-64 / TS16 - Organizations API Base

## Prerequisites

- APPWEB-51 foundation is merged into `develop`.
- MySQL is running locally.
- The API uses the local `root` / `root` credentials and `coldtrace_platform` database.
- EF Core migrations are applied on startup.

## Manual Checks

1. Restore dependencies from Rider or the .NET SDK configured for `net10.0`.
2. Run the API using the `http` launch profile.
3. Open `/swagger/index.html`.
4. Confirm Swagger shows the `Organizations` tag.
5. Run `GET /api/v1/organizations` and confirm it returns `200 OK` with an array.
6. Run `POST /api/v1/organizations` with:

```json
{
  "legalName": "ColdTrace Demo SAC",
  "commercialName": "ColdTrace Demo",
  "taxId": "20123456789",
  "contactEmail": "admin@coldtrace.demo"
}
```

7. Confirm the response is `201 Created` with a backend-generated `id`.
8. Repeat the same request and confirm the response is `409 Conflict`.
9. Confirm `POST /api/v1/organization-sign-ups` is not implemented in this ticket.

## Out of Scope

- Organization sign-up with first user.
- Login, sessions, JWT, and authorization.
- Organization resolution from an authenticated session.
- Organization profile update.
