using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI resolution plan rejection result.
/// </summary>
public static class ActionResultFromRejectAiResolutionPlanResultAssembler
{
    public static ActionResult ToActionResultFromRejectAiResolutionPlanResult(
        Result<AiResolutionPlan, RejectAiResolutionPlanError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AiResolutionPlan, RejectAiResolutionPlanError>.Success success =>
                controller.Ok(AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AiResolutionPlan, RejectAiResolutionPlanError>.Failure failure =>
                failure.Error switch
                {
                    RejectAiResolutionPlanError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    RejectAiResolutionPlanError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    RejectAiResolutionPlanError.PlanNotFound =>
                        controller.NotFound(localizer["AiResolutionPlanNotFound"].Value),
                    RejectAiResolutionPlanError.PlanAlreadyDecided =>
                        controller.Conflict(localizer["AiResolutionPlanAlreadyDecided"].Value),
                    RejectAiResolutionPlanError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorRejectingAiResolutionPlan"].Value,
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
