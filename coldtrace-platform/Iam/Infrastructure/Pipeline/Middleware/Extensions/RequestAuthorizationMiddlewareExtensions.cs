using ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Registers the IAM request authorization middleware.
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder) =>
        builder.UseMiddleware<RequestAuthorizationMiddleware>();
}
