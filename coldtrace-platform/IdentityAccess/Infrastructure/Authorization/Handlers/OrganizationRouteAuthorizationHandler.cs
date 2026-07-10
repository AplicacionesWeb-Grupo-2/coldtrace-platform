using System.Globalization;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Claims;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Handlers;

/// <summary>
///     Authorizes organization routes only when their identifier matches the JWT organization claim.
/// </summary>
public sealed class OrganizationRouteAuthorizationHandler : AuthorizationHandler<OrganizationRouteRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrganizationRouteRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext ||
            !httpContext.Request.RouteValues.TryGetValue(OrganizationRouteRequirement.RouteValueName,
                out var routeValue))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var routeOrganizationId = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
        var claimOrganizationId = context.User.FindFirst(ColdTraceClaimTypes.OrganizationId)?.Value;

        if (int.TryParse(routeOrganizationId, NumberStyles.None, CultureInfo.InvariantCulture,
                out var requestedOrganizationId) &&
            int.TryParse(claimOrganizationId, NumberStyles.None, CultureInfo.InvariantCulture,
                out var authenticatedOrganizationId) &&
            requestedOrganizationId == authenticatedOrganizationId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
