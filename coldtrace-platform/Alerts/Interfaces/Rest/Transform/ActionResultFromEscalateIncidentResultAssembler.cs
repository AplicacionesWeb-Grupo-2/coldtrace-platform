using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident escalation result.
/// </summary>
public static class ActionResultFromEscalateIncidentResultAssembler
{
    public static ActionResult ToActionResultFromEscalateIncidentResult(
        Result<Incident, EscalateIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<Incident, EscalateIncidentError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, EscalateIncidentError>.Failure failure =>
                failure.Error switch
                {
                    EscalateIncidentError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    EscalateIncidentError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    EscalateIncidentError.AlreadyResolved =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyResolved", StatusCodes.Status409Conflict),
                    EscalateIncidentError.AlreadyEscalated =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyEscalated", StatusCodes.Status409Conflict),
                    EscalateIncidentError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorEscalatingIncident", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
