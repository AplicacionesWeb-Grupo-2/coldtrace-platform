namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while escalating an incident.
/// </summary>
public enum EscalateIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    AlreadyResolved,
    AlreadyEscalated,
    UnexpectedError
}
