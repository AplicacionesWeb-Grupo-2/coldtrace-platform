namespace ColdTrace.Platform.Alerts.Domain.Model.Queries;

/// <summary>
///     Query for retrieving AI resolution plan history for one incident.
/// </summary>
public record GetAiResolutionPlansByIncidentIdAndOrganizationIdQuery(int OrganizationId, int IncidentId);
