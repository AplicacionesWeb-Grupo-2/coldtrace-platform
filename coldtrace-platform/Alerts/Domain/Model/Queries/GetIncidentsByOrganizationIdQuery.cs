namespace ColdTrace.Platform.Alerts.Domain.Model.Queries;

/// <summary>
///     Query for retrieving incidents owned by an organization.
/// </summary>
public record GetIncidentsByOrganizationIdQuery(int OrganizationId);
