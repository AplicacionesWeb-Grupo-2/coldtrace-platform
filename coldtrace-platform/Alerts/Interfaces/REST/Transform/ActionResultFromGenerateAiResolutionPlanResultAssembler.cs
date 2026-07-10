using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI resolution plan generation result.
/// </summary>
public static class ActionResultFromGenerateAiResolutionPlanResultAssembler
{
    public static ActionResult ToActionResultFromGenerateAiResolutionPlanResult(
        Result<AiResolutionPlan, GenerateAiResolutionPlanError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AiResolutionPlan, GenerateAiResolutionPlanError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AiResolutionPlan, GenerateAiResolutionPlanError>.Failure failure =>
                failure.Error switch
                {
                    GenerateAiResolutionPlanError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GenerateAiResolutionPlanError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    GenerateAiResolutionPlanError.IncidentCannotReceivePlans =>
                        controller.ProblemResponse(localizer, "IncidentCannotReceiveAiResolutionPlan", StatusCodes.Status409Conflict),
                    GenerateAiResolutionPlanError.IncidentContextUnavailable =>
                        controller.ProblemResponse(localizer, "IncidentAiContextUnavailable", StatusCodes.Status409Conflict),
                    GenerateAiResolutionPlanError.AiProviderDisabled =>
                        controller.ProblemResponse(localizer, "AiProviderDisabled", StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderNotConfigured =>
                        controller.ProblemResponse(localizer, "AiProviderNotConfigured", StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderUnavailable =>
                        controller.ProblemResponse(localizer, "AiProviderRequestFailed", StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderTimeout =>
                        controller.ProblemResponse(localizer, "AiProviderTimedOut", StatusCodes.Status504GatewayTimeout),
                    GenerateAiResolutionPlanError.InvalidStructuredOutput =>
                        controller.ProblemResponse(localizer, "AiResolutionPlanInvalidStructuredOutput", StatusCodes.Status502BadGateway),
                    GenerateAiResolutionPlanError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGeneratingAiResolutionPlan", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
