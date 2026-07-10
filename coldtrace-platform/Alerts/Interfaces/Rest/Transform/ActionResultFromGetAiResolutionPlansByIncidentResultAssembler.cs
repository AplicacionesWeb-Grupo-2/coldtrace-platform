using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident AI resolution plan history query result.
/// </summary>
public static class ActionResultFromGetAiResolutionPlansByIncidentResultAssembler
{
    public static ActionResult ToActionResultFromGetAiResolutionPlansByIncidentResult(
        Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Success success =>
                controller.Ok(success.Value.Select(AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Failure failure =>
                failure.Error switch
                {
                    GetAiResolutionPlansByIncidentError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetAiResolutionPlansByIncidentError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    GetAiResolutionPlansByIncidentError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingAiResolutionPlans", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
