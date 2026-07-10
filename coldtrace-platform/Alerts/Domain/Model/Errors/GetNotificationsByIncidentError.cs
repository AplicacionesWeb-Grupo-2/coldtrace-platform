namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving incident notifications.
/// </summary>
public enum GetNotificationsByIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    UnexpectedError
}
