namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving organization incidents.
/// </summary>
public enum GetIncidentsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
