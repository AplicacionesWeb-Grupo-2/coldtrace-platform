namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving one incident.
/// </summary>
public enum GetIncidentByIdAndOrganizationError
{
    OrganizationNotFound,
    IncidentNotFound,
    UnexpectedError
}
