namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving incident notifications.
/// </summary>
public enum GetNotificationsByIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    UnexpectedError
}
