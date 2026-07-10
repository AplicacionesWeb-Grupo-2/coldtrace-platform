namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

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
