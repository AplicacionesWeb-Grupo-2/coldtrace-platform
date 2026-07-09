using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using MvcProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;
using MvcValidationProblemDetails = Microsoft.AspNetCore.Mvc.ValidationProblemDetails;

namespace ColdTrace.Platform.Shared.Interfaces.REST.ProblemDetails;

/// <summary>
///     Creates the shared RFC 7807 response shapes used by controllers and MVC validation.
/// </summary>
public static class RestProblemDetailsFactory
{
    private const string ProblemJsonContentType = "application/problem+json";

    public static ObjectResult CreateProblemResponse(
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer,
        string detailResourceKey,
        int statusCode,
        string? code = null)
    {
        var problemDetails = BuildProblemDetails(
            controller.HttpContext,
            localizer,
            detailResourceKey,
            statusCode,
            code);
        var result = new ObjectResult(problemDetails) { StatusCode = statusCode };
        result.ContentTypes.Add(ProblemJsonContentType);
        return result;
    }

    public static BadRequestObjectResult CreateValidationProblemResponse(
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer,
        string detailResourceKey)
    {
        var message = localizer[detailResourceKey].Value;
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["request"] = [message]
        };
        return CreateValidationProblemResponse(
            controller.HttpContext,
            localizer,
            errors,
            detailResourceKey,
            RestErrorCodes.ValidationError);
    }

    public static IActionResult CreateValidationProblemResponse(ActionContext actionContext)
    {
        var localizer = actionContext.HttpContext.RequestServices
            .GetRequiredService<IStringLocalizer<SharedResource>>();
        var errors = ToValidationErrors(actionContext.ModelState, localizer);
        return CreateValidationProblemResponse(
            actionContext.HttpContext,
            localizer,
            errors,
            "ValidationFailedDetail",
            RestErrorCodes.ValidationError);
    }

    public static MvcProblemDetails BuildProblemDetails(
        HttpContext httpContext,
        IStringLocalizer<SharedResource> localizer,
        string detailResourceKey,
        int statusCode,
        string? code = null) =>
        new()
        {
            Status = statusCode,
            Title = localizer[TitleResourceKey(statusCode)].Value,
            Detail = localizer[detailResourceKey].Value,
            Instance = RequestInstance(httpContext),
            Extensions = { ["code"] = code ?? RestErrorCodes.FromResourceKey(detailResourceKey) }
        };

    public static MvcValidationProblemDetails BuildValidationProblemDetails(
        HttpContext httpContext,
        IStringLocalizer<SharedResource> localizer,
        IDictionary<string, string[]> errors,
        string detailResourceKey = "ValidationFailedDetail",
        string code = RestErrorCodes.ValidationError) =>
        new(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = localizer["ValidationFailedTitle"].Value,
            Detail = localizer[detailResourceKey].Value,
            Instance = RequestInstance(httpContext),
            Extensions = { ["code"] = code }
        };

    public static void ApplyDefaults(ProblemDetailsContext context)
    {
        var localizer = context.HttpContext.RequestServices
            .GetRequiredService<IStringLocalizer<SharedResource>>();
        var problemDetails = context.ProblemDetails;
        var statusCode = problemDetails.Status ?? context.HttpContext.Response.StatusCode;

        problemDetails.Status = statusCode;
        problemDetails.Instance = string.IsNullOrWhiteSpace(problemDetails.Instance)
            ? RequestInstance(context.HttpContext)
            : problemDetails.Instance;

        if (context.Exception is not null && statusCode >= StatusCodes.Status500InternalServerError)
        {
            problemDetails.Title = localizer["UnexpectedServerError"].Value;
            problemDetails.Detail = localizer["UnexpectedErrorProcessingRequest"].Value;
            problemDetails.Extensions["code"] = RestErrorCodes.UnexpectedError;
            return;
        }

        var hasCode = problemDetails.Extensions.ContainsKey("code");
        if (!hasCode)
        {
            problemDetails.Title = localizer[TitleResourceKey(statusCode)].Value;
            problemDetails.Detail = localizer[DetailResourceKey(statusCode)].Value;
        }
        else if (string.IsNullOrWhiteSpace(problemDetails.Detail))
        {
            problemDetails.Title = localizer[TitleResourceKey(statusCode)].Value;
            problemDetails.Detail = localizer[DetailResourceKey(statusCode)].Value;
        }
        else if (string.IsNullOrWhiteSpace(problemDetails.Title))
        {
            problemDetails.Title = localizer[TitleResourceKey(statusCode)].Value;
        }

        if (!problemDetails.Extensions.ContainsKey("code"))
            problemDetails.Extensions["code"] = RestErrorCodes.FromStatusCode(statusCode);
    }

    private static BadRequestObjectResult CreateValidationProblemResponse(
        HttpContext httpContext,
        IStringLocalizer<SharedResource> localizer,
        IDictionary<string, string[]> errors,
        string detailResourceKey,
        string code)
    {
        var problemDetails = BuildValidationProblemDetails(
            httpContext,
            localizer,
            errors,
            detailResourceKey,
            code);
        var result = new BadRequestObjectResult(problemDetails);
        result.ContentTypes.Add(ProblemJsonContentType);
        return result;
    }

    private static Dictionary<string, string[]> ToValidationErrors(
        ModelStateDictionary modelState,
        IStringLocalizer<SharedResource> localizer)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var (field, entry) in modelState)
        {
            if (entry.Errors.Count == 0) continue;

            var key = string.IsNullOrWhiteSpace(field) ? "request" : field;
            var messages = entry.Errors
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? localizer["InvalidField"].Value
                    : error.ErrorMessage)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            errors[key] = errors.TryGetValue(key, out var existing)
                ? existing.Concat(messages).Distinct(StringComparer.Ordinal).ToArray()
                : messages;
        }

        if (errors.Count == 0) errors["request"] = [localizer["InvalidField"].Value];
        return errors;
    }

    private static string TitleResourceKey(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "BadRequestTitle",
            StatusCodes.Status401Unauthorized => "UnauthorizedTitle",
            StatusCodes.Status403Forbidden => "ForbiddenTitle",
            StatusCodes.Status404NotFound => "NotFoundTitle",
            StatusCodes.Status405MethodNotAllowed => "MethodNotAllowedTitle",
            StatusCodes.Status409Conflict => "ConflictTitle",
            StatusCodes.Status415UnsupportedMediaType => "UnsupportedMediaTypeTitle",
            StatusCodes.Status502BadGateway => "BadGatewayTitle",
            StatusCodes.Status503ServiceUnavailable => "ServiceUnavailableTitle",
            StatusCodes.Status504GatewayTimeout => "GatewayTimeoutTitle",
            _ => "UnexpectedServerError"
        };

    private static string DetailResourceKey(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "InvalidRequest",
            StatusCodes.Status401Unauthorized => "AuthenticationRequired",
            StatusCodes.Status403Forbidden => "AccessForbidden",
            StatusCodes.Status404NotFound => "ResourceNotFound",
            StatusCodes.Status405MethodNotAllowed => "MethodNotAllowed",
            StatusCodes.Status409Conflict => "ResourceConflict",
            StatusCodes.Status415UnsupportedMediaType => "UnsupportedMediaType",
            _ => "UnexpectedErrorProcessingRequest"
        };

    private static string RequestInstance(HttpContext httpContext) =>
        httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value! : "/";
}
