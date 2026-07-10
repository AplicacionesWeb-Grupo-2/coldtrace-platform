namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while resolving an incident.
/// </summary>
public enum ResolveIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    AlreadyResolved,
    InvalidLifecycleTransition,
    UnexpectedError
}
