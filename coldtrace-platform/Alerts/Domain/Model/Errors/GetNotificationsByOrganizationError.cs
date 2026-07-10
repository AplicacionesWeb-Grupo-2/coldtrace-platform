namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving organization notifications.
/// </summary>
public enum GetNotificationsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
