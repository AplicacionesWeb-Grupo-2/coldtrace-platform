using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident detail query result.
/// </summary>
public static class ActionResultFromGetIncidentByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetIncidentByIdResult(
        Result<Incident, GetIncidentByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<Incident, GetIncidentByIdAndOrganizationError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, GetIncidentByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIncidentByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetIncidentByIdAndOrganizationError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    GetIncidentByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingIncidents", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
