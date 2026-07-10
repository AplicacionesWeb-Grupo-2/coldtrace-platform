using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ColdTrace.Platform.Billing.Interfaces.Acl;

/// <summary>
///     Writes the centralized conflict response for subscription entitlement failures.
/// </summary>
public sealed class PlanLimitExceededExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not PlanLimitExceededException) return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = StatusCodes.Status409Conflict }
        });
    }
}
