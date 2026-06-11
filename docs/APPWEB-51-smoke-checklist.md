# APPWEB-51 Smoke Checklist

Ticket: T-44 - ASP.NET Core API Foundation

## Prerequisites

- MySQL is running locally.
- The local schema exists:

```sql
CREATE DATABASE IF NOT EXISTS coldtrace_platform;
```

- The local connection string matches `coldtrace-platform/appsettings.Development.json`.

## Manual Checks

1. Restore dependencies from Rider or the .NET SDK configured for `net10.0`.
2. Run the API using the `http` or `https` launch profile.
3. Confirm the app starts and applies pending EF Core migrations without errors.
4. Open `/swagger/index.html`.
5. Open `/swagger/v1/swagger.json`.
6. Confirm future bounded contexts can be added under:
   - `Domain/Model`
   - `Application`
   - `Infrastructure/Persistence/EFC/Repositories`
   - `Interfaces/REST/Resources`
   - `Interfaces/REST/Transform`

## Out of Scope

- Real sign-in.
- JWT or session generation.
- Authorization enforcement.
- Production deployment.
- Automated tests.
