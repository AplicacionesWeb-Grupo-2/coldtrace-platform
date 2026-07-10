# APPWEB-65 / T-58 JWT And CORS Smoke Checklist

## Preconditions

- MySQL is available and the API can apply its migrations.
- `JWT_SECRET` contains a private value of at least 32 bytes.
- `CORS_ALLOWED_ORIGINS` contains comma-separated exact frontend origins outside Development.
- The test account uses a non-default password.

Run locally with an ephemeral development signing secret:

```bash
export JWT_SECRET="$(openssl rand -base64 48)"
ASPNETCORE_ENVIRONMENT=Development \
  "$HOME/.dotnet/dotnet" run \
  --project coldtrace-platform/coldtrace-platform.csproj \
  --urls http://localhost:5271
```

## Intentional Public Surface

- `POST /api/v1/authentication/sign-in`
- `POST /api/v1/organization-sign-ups`
- `GET /api/v1/subscription-plans`
- `POST /api/v1/billing/stripe/webhooks` (authenticated by Stripe signature validation)
- `/swagger/index.html`, `/swagger/**`, and the OpenAPI JSON asset
- CORS `OPTIONS` preflight requests, which expose no resource data

There is no health endpoint on this branch, so no health route is added to the anonymous surface.

## Authentication Checks

1. Request `GET /swagger/v1/swagger.json` without a token and expect `200`; confirm `bearerAuth` is an HTTP bearer JWT scheme.
2. Request `GET /api/v1/subscription-plans` without a token and expect `200`.
3. Request `GET /api/v1/roles` without a token and expect `401`, `Content-Type: application/problem+json`, and a body with `status: 401`.
4. Repeat with a malformed or wrong-signature bearer token and expect the same `401` contract.
5. Create the first tenant through `POST /api/v1/organization-sign-ups` with a non-default password.
6. Sign in through `POST /api/v1/authentication/sign-in`, copy the returned `token`, and request `GET /api/v1/roles` with `Authorization: Bearer <token>`; expect `200`.
7. Call an organization-scoped route with a token from another organization and confirm it returns `403`.
8. With a valid token, exceed a base-plan limit and confirm the existing `409` response still contains the plan-entitlement properties.

## CORS Checks

Allowed origin:

```bash
curl -i -X OPTIONS http://localhost:5271/api/v1/roles \
  -H 'Origin: http://localhost:5173' \
  -H 'Access-Control-Request-Method: GET'
```

Expect `204` and `Access-Control-Allow-Origin: http://localhost:5173`. Repeat with an unlisted origin and confirm the response does not include `Access-Control-Allow-Origin`.

## Automated Verification

```bash
"$HOME/.dotnet/dotnet" build coldtrace-platform.sln
git diff --check
```
