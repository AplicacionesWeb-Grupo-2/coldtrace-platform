# ColdTrace Platform

ASP.NET Core backend for the ColdTrace Aplicaciones Web project.

## Stack

- C#
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Swagger/OpenAPI

## Branching

- `main`: stable branch
- `develop`: integration branch
- `feature/t-44-api-foundation`: current APPWEB-51 work branch

## Local Setup

Create the local MySQL schema before running the API:

```sql
CREATE DATABASE IF NOT EXISTS coldtrace_platform;
```

The development connection string is configured in:

```text
coldtrace-platform/appsettings.Development.json
```

The default local MySQL credentials are `root` / `root`. If your local MySQL
password is different, override it in the Rider run configuration instead of
committing a personal password:

```text
ConnectionStrings__DefaultConnection=server=localhost;user=root;password=<your-password>;database=coldtrace_platform
```

Use the .NET 10 SDK configured under `~/.dotnet/dotnet`. The system `dotnet`
under `/usr/local/share/dotnet` only has older SDKs on this machine.

Manual validation for APPWEB-51 is documented in:

```text
docs/APPWEB-51-smoke-checklist.md
```
