using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident escalation result.
/// </summary>
public static class ActionResultFromEscalateIncidentResultAssembler
{
    public static ActionResult ToActionResultFromEscalateIncidentResult(
        Result<Incident, EscalateIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Incident, EscalateIncidentError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, EscalateIncidentError>.Failure failure =>
                failure.Error switch
                {
                    EscalateIncidentError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    EscalateIncidentError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    EscalateIncidentError.AlreadyResolved =>
                        controller.Conflict(localizer["IncidentAlreadyResolved"].Value),
                    EscalateIncidentError.AlreadyEscalated =>
                        controller.Conflict(localizer["IncidentAlreadyEscalated"].Value),
                    EscalateIncidentError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorEscalatingIncident"].Value,
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
