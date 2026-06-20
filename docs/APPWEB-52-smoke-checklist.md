# APPWEB-52 Smoke Checklist

Ticket: T-45 / TS01 - Organization Sign-Up API

## Prerequisites

- APPWEB-73 organizations API base is merged into `develop`.
- MySQL is running locally.
- The API uses the local `root` / `root` credentials and `coldtrace_platform` database.
- EF Core migrations are applied on startup.

## Manual Checks

1. Restore dependencies from Rider or the .NET SDK configured for `net10.0`.
2. Run the API using the `http` or `https` launch profile.
3. Open `/swagger/index.html`.
4. Confirm Swagger shows the `Organization Sign-Ups` tag.
5. Run `POST /api/v1/organization-sign-ups` with:

```json
{
  "legalName": "ColdTrace Sign Up SAC",
  "commercialName": "ColdTrace Sign Up",
  "taxId": "20987654321",
  "contactEmail": "owner@signup.demo",
  "firstName": "Mauricio",
  "lastName": "Pajes",
  "email": "mauricio@signup.demo"
}
```

6. Confirm the response is `201 Created` with one `organization` and one `user`.
7. Confirm the created user has the seeded `super-admin` role id.
8. Repeat the same request and confirm the response is `409 Conflict`.
9. Send a request with a different organization email and tax id but the same user email, and confirm the response is `409 Conflict`.
10. If a local MySQL schema from an older branch blocks startup, drop `coldtrace_platform` and run again so EF applies the current migrations from scratch.

## Out of Scope

- Real sign-in.
- Password hashing.
- JWT or session generation.
- Email verification.
- Authorization enforcement.
