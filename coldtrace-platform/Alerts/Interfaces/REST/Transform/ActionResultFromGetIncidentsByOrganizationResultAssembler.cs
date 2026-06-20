using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization incidents query result.
/// </summary>
public static class ActionResultFromGetIncidentsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetIncidentsByOrganizationResult(
        Result<IEnumerable<Incident>, GetIncidentsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(IncidentResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIncidentsByOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetIncidentsByOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingIncidents"].Value,
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
