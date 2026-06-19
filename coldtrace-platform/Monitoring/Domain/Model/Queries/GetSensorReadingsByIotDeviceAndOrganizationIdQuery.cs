namespace ColdTrace.Platform.Monitoring.Domain.Model.Queries;

/// <summary>
///     Query for retrieving sensor readings by organization and IoT device.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="IotDeviceId">IoT device identifier.</param>
public record GetSensorReadingsByIotDeviceAndOrganizationIdQuery(int OrganizationId, int IotDeviceId);
