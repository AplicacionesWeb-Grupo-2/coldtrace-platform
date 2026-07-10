using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization incidents query result.
/// </summary>
public static class ActionResultFromGetIncidentsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetIncidentsByOrganizationResult(
        Result<IEnumerable<Incident>, GetIncidentsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(IncidentResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIncidentsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetIncidentsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingIncidents", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
