using Microsoft.AspNetCore.Authorization;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Requirements;

/// <summary>
///     Requires organization-scoped routes to match the authenticated user's organization.
/// </summary>
public sealed class OrganizationRouteRequirement : IAuthorizationRequirement
{
    public const string RouteValueName = "organizationId";
}
