using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an incident notifications query result.
/// </summary>
public static class ActionResultFromGetNotificationsByIncidentResultAssembler
{
    public static ActionResult ToActionResultFromGetNotificationsByIncidentResult(
        Result<IEnumerable<Notification>, GetNotificationsByIncidentError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Success success =>
                controller.Ok(success.Value.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Failure failure =>
                failure.Error switch
                {
                    GetNotificationsByIncidentError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetNotificationsByIncidentError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    GetNotificationsByIncidentError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingNotifications", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
