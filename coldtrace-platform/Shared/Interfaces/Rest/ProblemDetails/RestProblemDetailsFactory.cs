using System.Diagnostics;
using ColdTrace.Platform.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using MvcProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;
using MvcValidationProblemDetails = Microsoft.AspNetCore.Mvc.ValidationProblemDetails;

namespace ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;

/// <summary>
///     Creates the shared RFC 7807 response shapes used by controllers and MVC validation.
/// </summary>
public static class RestProblemDetailsFactory
{
    private const string ProblemJsonContentType = "application/problem+json";
    private const string CodeExtensionKey = "code";
    private const string TraceIdExtensionKey = "traceId";

    public static ObjectResult CreateProblemResponse(
        ControllerBase controller,
        IStringLocalizer localizer,
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
        IStringLocalizer localizer,
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
        IStringLocalizer localizer,
        string detailResourceKey,
        int statusCode,
        string? code = null) =>
        new()
        {
            Status = statusCode,
            Title = localizer[TitleResourceKey(statusCode)].Value,
            Detail = localizer[detailResourceKey].Value,
            Instance = RequestInstance(httpContext),
            Extensions =
            {
                [CodeExtensionKey] = code ?? RestErrorCodes.FromResourceKey(detailResourceKey),
                [TraceIdExtensionKey] = RequestTraceId(httpContext)
            }
        };

    public static MvcValidationProblemDetails BuildValidationProblemDetails(
        HttpContext httpContext,
        IStringLocalizer localizer,
        IDictionary<string, string[]> errors,
        string detailResourceKey = "ValidationFailedDetail",
        string code = RestErrorCodes.ValidationError) =>
        new(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = localizer["ValidationFailedTitle"].Value,
            Detail = localizer[detailResourceKey].Value,
            Instance = RequestInstance(httpContext),
            Extensions =
            {
                [CodeExtensionKey] = code,
                [TraceIdExtensionKey] = RequestTraceId(httpContext)
            }
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

        var hasFeatureCode = HasFeatureCode(problemDetails, statusCode);
        if (context.Exception is not null && !hasFeatureCode)
        {
            problemDetails.Title = localizer[TitleResourceKey(statusCode)].Value;
            problemDetails.Detail = localizer[DetailResourceKey(statusCode)].Value;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(problemDetails.Title))
                problemDetails.Title = localizer[TitleResourceKey(statusCode)].Value;

            if (string.IsNullOrWhiteSpace(problemDetails.Detail))
                problemDetails.Detail = localizer[DetailResourceKey(statusCode)].Value;
        }

        if (!HasExtensionValue(problemDetails, CodeExtensionKey))
            problemDetails.Extensions[CodeExtensionKey] = RestErrorCodes.FromStatusCode(statusCode);

        if (!HasExtensionValue(problemDetails, TraceIdExtensionKey))
            problemDetails.Extensions[TraceIdExtensionKey] = RequestTraceId(context.HttpContext);
    }

    private static BadRequestObjectResult CreateValidationProblemResponse(
        HttpContext httpContext,
        IStringLocalizer localizer,
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
        IStringLocalizer localizer)
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

    private static string RequestTraceId(HttpContext httpContext) =>
        Activity.Current?.Id ?? httpContext.TraceIdentifier;

    private static bool HasFeatureCode(MvcProblemDetails problemDetails, int statusCode)
    {
        if (!problemDetails.Extensions.TryGetValue(CodeExtensionKey, out var code) ||
            code is not string codeValue ||
            string.IsNullOrWhiteSpace(codeValue))
            return false;

        return !string.Equals(
            codeValue,
            RestErrorCodes.FromStatusCode(statusCode),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasExtensionValue(MvcProblemDetails problemDetails, string extensionKey) =>
        problemDetails.Extensions.TryGetValue(extensionKey, out var value) &&
        value switch
        {
            null => false,
            string stringValue => !string.IsNullOrWhiteSpace(stringValue),
            _ => true
        };
}
