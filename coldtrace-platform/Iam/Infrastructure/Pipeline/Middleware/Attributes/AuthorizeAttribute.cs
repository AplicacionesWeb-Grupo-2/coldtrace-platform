using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Requires the user loaded by the request authorization middleware.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    /// <inheritdoc />
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();
        if (allowAnonymous) return;

        if (context.HttpContext.Items["User"] is not User)
            context.Result = new UnauthorizedResult();
    }
}
