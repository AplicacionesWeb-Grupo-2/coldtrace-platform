namespace ColdTrace.Platform.Monitoring.Domain.Model.Queries;

/// <summary>
///     Query for retrieving sensor readings by organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetSensorReadingsByOrganizationIdQuery(int OrganizationId);
