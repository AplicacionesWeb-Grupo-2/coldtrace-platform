using ColdTrace.Platform.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;

public static class ControllerBaseExtensions
{
    public static ObjectResult ProblemResponse(
        this ControllerBase controller,
        IStringLocalizer localizer,
        string detailResourceKey,
        int statusCode,
        string? code = null) =>
        RestProblemDetailsFactory.CreateProblemResponse(
            controller,
            localizer,
            detailResourceKey,
            statusCode,
            code);

    public static BadRequestObjectResult ValidationProblemResponse(
        this ControllerBase controller,
        IStringLocalizer localizer,
        string detailResourceKey) =>
        RestProblemDetailsFactory.CreateValidationProblemResponse(
            controller,
            localizer,
            detailResourceKey);
}
