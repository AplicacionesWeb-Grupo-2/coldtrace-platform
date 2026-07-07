using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident AI resolution plan history query result.
/// </summary>
public static class ActionResultFromGetAiResolutionPlansByIncidentResultAssembler
{
    public static ActionResult ToActionResultFromGetAiResolutionPlansByIncidentResult(
        Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Success success =>
                controller.Ok(success.Value.Select(AiResolutionPlanResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Failure failure =>
                failure.Error switch
                {
                    GetAiResolutionPlansByIncidentError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetAiResolutionPlansByIncidentError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    GetAiResolutionPlansByIncidentError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingAiResolutionPlans"].Value,
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
