namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while acknowledging an incident.
/// </summary>
public enum AcknowledgeIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    AlreadyAcknowledged,
    AlreadyResolved,
    UnexpectedError
}
