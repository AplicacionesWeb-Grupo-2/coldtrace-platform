namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving organization incidents.
/// </summary>
public enum GetIncidentsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
