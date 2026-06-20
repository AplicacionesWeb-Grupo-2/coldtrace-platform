namespace ColdTrace.Platform.Alerts.Application.Errors;

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
