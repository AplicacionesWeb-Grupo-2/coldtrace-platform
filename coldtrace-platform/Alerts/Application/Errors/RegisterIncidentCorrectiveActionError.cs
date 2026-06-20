namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while registering corrective action for an incident.
/// </summary>
public enum RegisterIncidentCorrectiveActionError
{
    OrganizationNotFound,
    IncidentNotFound,
    AlreadyResolved,
    UnexpectedError
}
