namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while creating an incident.
/// </summary>
public enum CreateIncidentError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
