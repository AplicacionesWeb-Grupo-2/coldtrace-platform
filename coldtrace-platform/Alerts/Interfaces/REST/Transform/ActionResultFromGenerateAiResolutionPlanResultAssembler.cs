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
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GenerateAiResolutionPlanError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    GenerateAiResolutionPlanError.IncidentCannotReceivePlans =>
                        controller.Conflict(localizer["IncidentCannotReceiveAiResolutionPlan"].Value),
                    GenerateAiResolutionPlanError.IncidentContextUnavailable =>
                        controller.Conflict(localizer["IncidentAiContextUnavailable"].Value),
                    GenerateAiResolutionPlanError.AiProviderDisabled =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderDisabled"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderNotConfigured =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderNotConfigured"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderUnavailable =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderRequestFailed"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateAiResolutionPlanError.AiProviderTimeout =>
                        controller.Problem(
                            title: localizer["AiProviderTimeout"].Value,
                            detail: localizer["AiProviderTimedOut"].Value,
                            statusCode: StatusCodes.Status504GatewayTimeout),
                    GenerateAiResolutionPlanError.InvalidStructuredOutput =>
                        controller.Problem(
                            title: localizer["InvalidAiStructuredOutput"].Value,
                            detail: localizer["AiResolutionPlanInvalidStructuredOutput"].Value,
                            statusCode: StatusCodes.Status502BadGateway),
                    GenerateAiResolutionPlanError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGeneratingAiResolutionPlan"].Value,
                            statusCode: 500),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
