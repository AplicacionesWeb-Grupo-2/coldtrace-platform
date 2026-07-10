using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident acknowledgement result.
/// </summary>
public static class ActionResultFromAcknowledgeIncidentResultAssembler
{
    public static ActionResult ToActionResultFromAcknowledgeIncidentResult(
        Result<Incident, AcknowledgeIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<Incident, AcknowledgeIncidentError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, AcknowledgeIncidentError>.Failure failure =>
                failure.Error switch
                {
                    AcknowledgeIncidentError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    AcknowledgeIncidentError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    AcknowledgeIncidentError.AlreadyAcknowledged =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyAcknowledged", StatusCodes.Status409Conflict),
                    AcknowledgeIncidentError.AlreadyResolved =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyResolved", StatusCodes.Status409Conflict),
                    AcknowledgeIncidentError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorAcknowledgingIncident", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
