namespace ColdTrace.Platform.Alerts.Domain.Model.Queries;

/// <summary>
///     Query for retrieving notification read models derived from one incident.
/// </summary>
public record GetNotificationsByIncidentIdAndOrganizationIdQuery(int OrganizationId, int IncidentId);
