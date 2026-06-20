namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while creating an incident.
/// </summary>
public enum CreateIncidentError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
