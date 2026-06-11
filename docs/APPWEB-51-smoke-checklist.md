# APPWEB-51 Smoke Checklist

Ticket: T-44 - ASP.NET Core API Foundation

## Prerequisites

- MySQL is running locally.
- Rider is using the .NET 10 SDK from `~/.dotnet/dotnet`.
- The local schema exists:

```sql
CREATE DATABASE IF NOT EXISTS coldtrace_platform;
```

- The local connection string matches `coldtrace-platform/appsettings.Development.json`,
  or Rider overrides it with this environment variable:

```text
ConnectionStrings__DefaultConnection=server=localhost;user=root;password=<your-password>;database=coldtrace_platform
```

- EF Core migrations are versioned under
  `coldtrace-platform/Shared/Infrastructure/Persistence/EFC/Migrations`.
- The sensitive data logging warning is expected in `Development`; it should not
  be enabled for production credentials.

## Manual Checks

1. Restore dependencies from Rider or the .NET SDK configured for `net10.0`.
2. Run the API using the `http` or `https` launch profile.
3. Confirm the app starts and applies pending EF Core migrations without errors.
4. Open `/swagger/index.html`.
5. Open `/swagger/v1/swagger.json`.
6. Open the Bruno collection in `bruno/ColdTrace Platform`.
7. Run the `API Foundation / OpenAPI Document` request.
8. Run the `API Foundation / Swagger UI` request.
9. Confirm future bounded contexts can be added under:
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
