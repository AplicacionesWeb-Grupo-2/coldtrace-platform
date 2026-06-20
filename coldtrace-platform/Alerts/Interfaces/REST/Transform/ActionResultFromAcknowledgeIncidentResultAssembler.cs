using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident acknowledgement result.
/// </summary>
public static class ActionResultFromAcknowledgeIncidentResultAssembler
{
    public static ActionResult ToActionResultFromAcknowledgeIncidentResult(
        Result<Incident, AcknowledgeIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Incident, AcknowledgeIncidentError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, AcknowledgeIncidentError>.Failure failure =>
                failure.Error switch
                {
                    AcknowledgeIncidentError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    AcknowledgeIncidentError.IncidentNotFound =>
                        controller.NotFound(localizer["IncidentNotFound"].Value),
                    AcknowledgeIncidentError.AlreadyAcknowledged =>
                        controller.Conflict(localizer["IncidentAlreadyAcknowledged"].Value),
                    AcknowledgeIncidentError.AlreadyResolved =>
                        controller.Conflict(localizer["IncidentAlreadyResolved"].Value),
                    AcknowledgeIncidentError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorAcknowledgingIncident"].Value,
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
