using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI resolution plan approval result.
/// </summary>
public static class ActionResultFromApproveAiResolutionPlanResultAssembler
{
    public static ActionResult ToActionResultFromApproveAiResolutionPlanResult(
        Result<AiResolutionPlan, ApproveAiResolutionPlanError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AiResolutionPlan, ApproveAiResolutionPlanError>.Success success =>
                controller.Ok(AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AiResolutionPlan, ApproveAiResolutionPlanError>.Failure failure =>
                failure.Error switch
                {
                    ApproveAiResolutionPlanError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    ApproveAiResolutionPlanError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    ApproveAiResolutionPlanError.PlanNotFound =>
                        controller.NotFound(localizer["AiResolutionPlanNotFound"].Value),
                    ApproveAiResolutionPlanError.PlanAlreadyDecided =>
                        controller.Conflict(localizer["AiResolutionPlanAlreadyDecided"].Value),
                    ApproveAiResolutionPlanError.IncidentAlreadyResolved =>
                        controller.Conflict(localizer["IncidentAlreadyResolved"].Value),
                    ApproveAiResolutionPlanError.InvalidIncidentLifecycleTransition =>
                        controller.Conflict(localizer["InvalidIncidentLifecycleTransition"].Value),
                    ApproveAiResolutionPlanError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorApprovingAiResolutionPlan"].Value,
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
