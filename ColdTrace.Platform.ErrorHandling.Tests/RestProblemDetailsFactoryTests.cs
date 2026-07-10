using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Interfaces.REST.ProblemDetails;
using System.Globalization;
using System.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace ColdTrace.Platform.ErrorHandling.Tests;

public class RestProblemDetailsFactoryTests
{
    private readonly IStringLocalizer<SharedResource> _localizer = new TestSharedResourceLocalizer();

    [Theory]
    [InlineData("MaintenanceScheduleNotFound", "en", "Maintenance schedule was not found.")]
    [InlineData("MaintenanceScheduleNotFound", "es", "La programacion de mantenimiento no fue encontrada.")]
    [InlineData("TechnicalServiceRequestInvalidTransition", "es",
        "La transicion de ciclo de vida de la solicitud de servicio tecnico no es valida.")]
    [InlineData("The {0} field is required.", "es", "El campo {0} es obligatorio.")]
    public void SharedResources_ResolveLocalizedMessages(string key, string cultureName, string expected)
    {
        var resourceManager = new ResourceManager(typeof(SharedResource));

        var value = resourceManager.GetString(key, CultureInfo.GetCultureInfo(cultureName));

        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("OrganizationNotFound", "ORGANIZATION_NOT_FOUND")]
    [InlineData("IotDeviceUuidDuplicated", "IOT_DEVICE_UUID_DUPLICATED")]
    [InlineData("AiProviderTimedOut", "AI_PROVIDER_TIMED_OUT")]
    public void FromResourceKey_ReturnsStableUpperSnakeCase(string resourceKey, string expected)
    {
        Assert.Equal(expected, RestErrorCodes.FromResourceKey(resourceKey));
    }

    [Theory]
    [InlineData(StatusCodes.Status401Unauthorized, RestErrorCodes.Unauthorized)]
    [InlineData(StatusCodes.Status403Forbidden, RestErrorCodes.Forbidden)]
    [InlineData(StatusCodes.Status405MethodNotAllowed, RestErrorCodes.MethodNotAllowed)]
    [InlineData(StatusCodes.Status415UnsupportedMediaType, RestErrorCodes.UnsupportedMediaType)]
    public void FromStatusCode_ReturnsStableProtocolCode(int statusCode, string expected)
    {
        Assert.Equal(expected, RestErrorCodes.FromStatusCode(statusCode));
    }

    [Fact]
    public void BuildProblemDetails_IncludesRequiredFieldsAndStableCode()
    {
        var httpContext = CreateHttpContext("/api/v1/organizations/999/locations");

        var problemDetails = RestProblemDetailsFactory.BuildProblemDetails(
            httpContext,
            _localizer,
            "OrganizationNotFound",
            StatusCodes.Status404NotFound);

        Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("Resource not found", problemDetails.Title);
        Assert.Equal("Organization was not found.", problemDetails.Detail);
        Assert.Equal("/api/v1/organizations/999/locations", problemDetails.Instance);
        Assert.Equal("ORGANIZATION_NOT_FOUND", problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
    }

    [Fact]
    public void BuildValidationProblemDetails_IncludesErrorsAndValidationCode()
    {
        var httpContext = CreateHttpContext("/api/v1/organizations");
        var errors = new Dictionary<string, string[]>
        {
            ["ContactEmail"] = ["The ContactEmail field is not a valid e-mail address."]
        };

        var problemDetails = RestProblemDetailsFactory.BuildValidationProblemDetails(
            httpContext,
            _localizer,
            errors);

        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Validation failed", problemDetails.Title);
        Assert.Equal("One or more validation errors occurred.", problemDetails.Detail);
        Assert.Equal("/api/v1/organizations", problemDetails.Instance);
        Assert.Equal(RestErrorCodes.ValidationError, problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
        Assert.Equal(errors["ContactEmail"], problemDetails.Errors["ContactEmail"]);
    }

    [Fact]
    public void ApplyDefaults_SanitizesUnhandledExceptions()
    {
        var httpContext = CreateHttpContext("/api/v1/organizations");
        var exception = new InvalidOperationException("database-password=secret");
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception.Message,
            Detail = exception.ToString()
        };
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        };

        RestProblemDetailsFactory.ApplyDefaults(context);

        Assert.Equal("Unexpected server error", problemDetails.Title);
        Assert.Equal("An unexpected error occurred while processing your request.", problemDetails.Detail);
        Assert.Equal("/api/v1/organizations", problemDetails.Instance);
        Assert.Equal(RestErrorCodes.UnexpectedError, problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
        Assert.DoesNotContain("secret", problemDetails.Detail, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(nameof(InvalidOperationException), problemDetails.Detail, StringComparison.Ordinal);
    }

    [Fact]
    public void ApplyDefaults_LocalizesProtocolErrorsWithoutFeatureCode()
    {
        var httpContext = CreateHttpContext("/api/v1/roles");
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized
        };
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        RestProblemDetailsFactory.ApplyDefaults(context);

        Assert.Equal("Authentication required", problemDetails.Title);
        Assert.Equal("A valid bearer token is required to access this resource.", problemDetails.Detail);
        Assert.Equal(RestErrorCodes.Unauthorized, problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
    }

    [Theory]
    [InlineData(
        StatusCodes.Status503ServiceUnavailable,
        "AI provider unavailable",
        "AI assistance is disabled by configuration.",
        RestErrorCodes.ServiceUnavailable)]
    [InlineData(
        StatusCodes.Status409Conflict,
        "Deletion blocked",
        "The gateway cannot be deleted because dependent devices exist.",
        RestErrorCodes.ResourceConflict)]
    [InlineData(
        StatusCodes.Status500InternalServerError,
        "Password reset request failed",
        "The password reset request could not be created.",
        RestErrorCodes.UnexpectedError)]
    public void ApplyDefaults_PreservesExplicitControllerProblemFields(
        int statusCode,
        string title,
        string detail,
        string expectedCode)
    {
        var httpContext = CreateHttpContext("/api/v1/original");
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = "https://coldtrace.example/problems/feature-error",
            Instance = "/api/v1/custom-instance"
        };
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        RestProblemDetailsFactory.ApplyDefaults(context);

        Assert.Equal(statusCode, problemDetails.Status);
        Assert.Equal(title, problemDetails.Title);
        Assert.Equal(detail, problemDetails.Detail);
        Assert.Equal("https://coldtrace.example/problems/feature-error", problemDetails.Type);
        Assert.Equal("/api/v1/custom-instance", problemDetails.Instance);
        Assert.Equal(expectedCode, problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
    }

    [Fact]
    public void ApplyDefaults_PreservesExplicitMappedExceptionWithFeatureCode()
    {
        var httpContext = CreateHttpContext("/api/v1/organizations/1/users");
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Current subscription plan does not allow creating another user",
            Detail = "Current subscription plan does not allow creating another user",
            Extensions = { ["code"] = "USER_PLAN_LIMIT_EXCEEDED" }
        };
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = new InvalidOperationException("mapped feature exception")
        };

        RestProblemDetailsFactory.ApplyDefaults(context);

        Assert.Equal("Current subscription plan does not allow creating another user", problemDetails.Title);
        Assert.Equal("Current subscription plan does not allow creating another user", problemDetails.Detail);
        Assert.Equal("USER_PLAN_LIMIT_EXCEEDED", problemDetails.Extensions["code"]);
        Assert.Equal("test-trace-id", problemDetails.Extensions["traceId"]);
    }

    [Fact]
    public void ApplyDefaults_PreservesExistingProblemExtensions()
    {
        var httpContext = CreateHttpContext("/api/v1/organizations/1/users");
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Current subscription plan does not allow creating another user",
            Detail = "Current subscription plan does not allow creating another user",
            Extensions =
            {
                ["code"] = "USER_PLAN_LIMIT_EXCEEDED",
                ["entitlementKey"] = "max_users",
                ["traceId"] = "upstream-trace-id"
            }
        };
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        RestProblemDetailsFactory.ApplyDefaults(context);

        Assert.Equal("USER_PLAN_LIMIT_EXCEEDED", problemDetails.Extensions["code"]);
        Assert.Equal("max_users", problemDetails.Extensions["entitlementKey"]);
        Assert.Equal("Current subscription plan does not allow creating another user", problemDetails.Detail);
        Assert.Equal("/api/v1/organizations/1/users", problemDetails.Instance);
        Assert.Equal("upstream-trace-id", problemDetails.Extensions["traceId"]);
    }

    private DefaultHttpContext CreateHttpContext(string path)
    {
        var services = new ServiceCollection()
            .AddSingleton(_localizer)
            .BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = services };
        httpContext.Request.Path = path;
        httpContext.TraceIdentifier = "test-trace-id";
        return httpContext;
    }

    private sealed class TestSharedResourceLocalizer : IStringLocalizer<SharedResource>
    {
        private static readonly IReadOnlyDictionary<string, string> Messages =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["UnexpectedServerError"] = "Unexpected server error",
                ["UnexpectedErrorProcessingRequest"] =
                    "An unexpected error occurred while processing your request.",
                ["NotFoundTitle"] = "Resource not found",
                ["UnauthorizedTitle"] = "Authentication required",
                ["AuthenticationRequired"] = "A valid bearer token is required to access this resource.",
                ["OrganizationNotFound"] = "Organization was not found.",
                ["ValidationFailedTitle"] = "Validation failed",
                ["ValidationFailedDetail"] = "One or more validation errors occurred."
            };

        public LocalizedString this[string name] =>
            new(name, Messages.GetValueOrDefault(name, name), !Messages.ContainsKey(name));

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(this[name].Value, arguments), !Messages.ContainsKey(name));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            Messages.Select(pair => new LocalizedString(pair.Key, pair.Value));
    }
}
