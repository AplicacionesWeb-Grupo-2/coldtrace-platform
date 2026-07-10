namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

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
