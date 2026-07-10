using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization notifications query result.
/// </summary>
public static class ActionResultFromGetNotificationsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetNotificationsByOrganizationResult(
        Result<IEnumerable<Notification>, GetNotificationsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(NotificationResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetNotificationsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetNotificationsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingNotifications", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
