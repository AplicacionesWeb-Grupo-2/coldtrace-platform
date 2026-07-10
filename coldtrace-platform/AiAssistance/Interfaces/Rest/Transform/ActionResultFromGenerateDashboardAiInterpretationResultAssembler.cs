using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest.Transform;

/// <summary>
///     Maps dashboard AI interpretation results to controlled HTTP responses.
/// </summary>
public static class ActionResultFromGenerateDashboardAiInterpretationResultAssembler
{
    public static ActionResult ToActionResultFromGenerationResult(
        Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError> result,
        ControllerBase controller,
        IStringLocalizer<AiAssistanceMessages> localizer) =>
        result switch
        {
            Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>.Success success =>
                controller.Ok(DashboardAiInterpretationResourceFromResultAssembler.ToResourceFromResult(success.Value)),
            Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>.Failure failure =>
                ToFailureResult(failure.Error, controller, localizer),
            _ => UnexpectedError(controller, localizer)
        };

    private static ActionResult ToFailureResult(
        GenerateDashboardAiInterpretationError error,
        ControllerBase controller,
        IStringLocalizer<AiAssistanceMessages> localizer) =>
        error switch
        {
            GenerateDashboardAiInterpretationError.OrganizationNotFound =>
                Problem(
                    controller,
                    localizer["OrganizationNotFound"].Value,
                    localizer["OrganizationNotFound"].Value,
                    StatusCodes.Status404NotFound),
            GenerateDashboardAiInterpretationError.DashboardContextUnavailable =>
                Problem(
                    controller,
                    localizer["UnexpectedServerError"].Value,
                    localizer["DashboardAiInterpretationContextUnavailable"].Value,
                    StatusCodes.Status500InternalServerError),
            GenerateDashboardAiInterpretationError.AiProviderDisabled =>
                Problem(
                    controller,
                    localizer["AiProviderUnavailable"].Value,
                    localizer["AiProviderDisabled"].Value,
                    StatusCodes.Status503ServiceUnavailable),
            GenerateDashboardAiInterpretationError.AiProviderNotConfigured =>
                Problem(
                    controller,
                    localizer["AiProviderUnavailable"].Value,
                    localizer["AiProviderNotConfigured"].Value,
                    StatusCodes.Status503ServiceUnavailable),
            GenerateDashboardAiInterpretationError.AiProviderUnavailable =>
                Problem(
                    controller,
                    localizer["AiProviderUnavailable"].Value,
                    localizer["AiProviderRequestFailed"].Value,
                    StatusCodes.Status503ServiceUnavailable),
            GenerateDashboardAiInterpretationError.AiProviderTimeout =>
                Problem(
                    controller,
                    localizer["AiProviderTimeout"].Value,
                    localizer["AiProviderTimedOut"].Value,
                    StatusCodes.Status504GatewayTimeout),
            GenerateDashboardAiInterpretationError.InvalidStructuredOutput =>
                Problem(
                    controller,
                    localizer["InvalidAiStructuredOutput"].Value,
                    localizer["DashboardAiInterpretationInvalidStructuredOutput"].Value,
                    StatusCodes.Status502BadGateway),
            _ => UnexpectedError(controller, localizer)
        };

    private static ObjectResult UnexpectedError(
        ControllerBase controller,
        IStringLocalizer<AiAssistanceMessages> localizer) =>
        Problem(
            controller,
            localizer["UnexpectedServerError"].Value,
            localizer["UnexpectedErrorGeneratingDashboardAiInterpretation"].Value,
            StatusCodes.Status500InternalServerError);

    private static ObjectResult Problem(
        ControllerBase controller,
        string title,
        string detail,
        int statusCode) =>
        controller.Problem(title: title, detail: detail, statusCode: statusCode);
}
