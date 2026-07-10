namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving AI resolution plan history for one incident.
/// </summary>
public enum GetAiResolutionPlansByIncidentError
{
    OrganizationNotFound,
    IncidentNotFound,
    UnexpectedError
}
