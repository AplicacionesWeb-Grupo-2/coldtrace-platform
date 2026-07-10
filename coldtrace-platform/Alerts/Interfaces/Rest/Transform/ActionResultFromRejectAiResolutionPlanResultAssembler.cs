using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI resolution plan rejection result.
/// </summary>
public static class ActionResultFromRejectAiResolutionPlanResultAssembler
{
    public static ActionResult ToActionResultFromRejectAiResolutionPlanResult(
        Result<AiResolutionPlan, RejectAiResolutionPlanError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<AiResolutionPlan, RejectAiResolutionPlanError>.Success success =>
                controller.Ok(AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AiResolutionPlan, RejectAiResolutionPlanError>.Failure failure =>
                failure.Error switch
                {
                    RejectAiResolutionPlanError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    RejectAiResolutionPlanError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    RejectAiResolutionPlanError.PlanNotFound =>
                        controller.ProblemResponse(localizer, "AiResolutionPlanNotFound", StatusCodes.Status404NotFound),
                    RejectAiResolutionPlanError.PlanAlreadyDecided =>
                        controller.ProblemResponse(localizer, "AiResolutionPlanAlreadyDecided", StatusCodes.Status409Conflict),
                    RejectAiResolutionPlanError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorRejectingAiResolutionPlan", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
