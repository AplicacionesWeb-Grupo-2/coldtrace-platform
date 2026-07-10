using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident creation result.
/// </summary>
public static class ActionResultFromCreateIncidentResultAssembler
{
    public static ActionResult ToActionResultFromCreateIncidentResult(
        Result<Incident, CreateIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Incident, CreateIncidentError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, CreateIncidentError>.Failure failure =>
                failure.Error switch
                {
                    CreateIncidentError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateIncidentError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    CreateIncidentError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingIncident", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
