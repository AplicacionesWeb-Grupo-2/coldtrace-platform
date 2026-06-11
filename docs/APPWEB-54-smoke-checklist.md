# APPWEB-54 Smoke Checklist

Ticket: T-47 / TS03 - Users API

## Prerequisites

- APPWEB-52 organization sign-up is merged into `develop`.
- MySQL is running locally.
- The API uses the local `root` / `root` credentials and `coldtrace_platform` database.
- EF Core migrations are applied on startup.

## Manual Checks

1. Restore dependencies from Rider or the .NET SDK configured for `net10.0`.
2. Run the API using the `http` or `https` launch profile.
3. Open `/swagger/index.html`.
4. Confirm Swagger shows `Users` and `Roles`.
5. Run `GET /api/v1/roles` and confirm it returns the seeded role catalog, including `super-admin`.
6. Create an organization with its first user using `POST /api/v1/organization-sign-ups`.
7. Run `GET /api/v1/organizations/{organizationId}/users` and confirm it returns only users for that organization.
8. Run `POST /api/v1/organizations/{organizationId}/users` with:

```json
{
  "firstName": "Andrea",
  "lastName": "Torres",
  "email": "andrea@signup.demo",
  "roleId": 2
}
```

9. Confirm the response is `201 Created` with a backend-generated `id`.
10. Repeat the same user request and confirm the response is `409 Conflict`.
11. Send the same request to a missing organization id and confirm `404 Not Found`.
12. Send the request with a missing role id and confirm `404 Not Found`.

## Out of Scope

- Role assignment/update for existing users.
- Real sign-in.
- Password hashing.
- JWT or session generation.
- Authorization enforcement.
