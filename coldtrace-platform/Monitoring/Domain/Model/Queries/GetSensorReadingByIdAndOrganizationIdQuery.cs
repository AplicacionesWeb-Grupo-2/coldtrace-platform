namespace ColdTrace.Platform.Monitoring.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one sensor reading by organization and id.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="SensorReadingId">Sensor reading identifier.</param>
public record GetSensorReadingByIdAndOrganizationIdQuery(int OrganizationId, int SensorReadingId);
