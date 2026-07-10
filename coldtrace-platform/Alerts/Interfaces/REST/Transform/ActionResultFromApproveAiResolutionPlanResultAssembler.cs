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
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    ApproveAiResolutionPlanError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    ApproveAiResolutionPlanError.PlanNotFound =>
                        controller.ProblemResponse(localizer, "AiResolutionPlanNotFound", StatusCodes.Status404NotFound),
                    ApproveAiResolutionPlanError.PlanAlreadyDecided =>
                        controller.ProblemResponse(localizer, "AiResolutionPlanAlreadyDecided", StatusCodes.Status409Conflict),
                    ApproveAiResolutionPlanError.IncidentAlreadyResolved =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyResolved", StatusCodes.Status409Conflict),
                    ApproveAiResolutionPlanError.InvalidIncidentLifecycleTransition =>
                        controller.ProblemResponse(localizer, "InvalidIncidentLifecycleTransition", StatusCodes.Status409Conflict),
                    ApproveAiResolutionPlanError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorApprovingAiResolutionPlan", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
