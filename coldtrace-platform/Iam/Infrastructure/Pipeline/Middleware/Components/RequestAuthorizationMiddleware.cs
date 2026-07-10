using ColdTrace.Platform.Iam.Application.Internal.OutboundServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Validates bearer tokens and loads the authenticated user into the request context.
/// </summary>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     Authorizes the current request using the Learning Center IAM pipeline pattern.
    /// </summary>
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').LastOrDefault();
        var userId = token is null ? null : await tokenService.ValidateToken(token);
        if (userId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var user = await userQueryService.Handle(new GetUserByIdQuery(userId.Value), context.RequestAborted);
        if (user is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (!MatchesOrganizationRoute(context, user))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        context.Items["User"] = user;
        await next(context);
    }

    private static bool MatchesOrganizationRoute(HttpContext context, User user)
    {
        if (!context.Request.RouteValues.TryGetValue("organizationId", out var routeValue)) return true;
        return int.TryParse(routeValue?.ToString(), out var organizationId) &&
               organizationId == user.OrganizationId;
    }
}
