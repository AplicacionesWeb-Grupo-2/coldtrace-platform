namespace ColdTrace.Platform.Alerts.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one organization incident.
/// </summary>
public record GetIncidentByIdAndOrganizationIdQuery(int OrganizationId, int IncidentId);
