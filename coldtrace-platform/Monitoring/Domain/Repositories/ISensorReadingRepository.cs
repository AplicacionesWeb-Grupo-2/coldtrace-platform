using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Monitoring.Domain.Repositories;

/// <summary>
///     Sensor reading repository contract.
/// </summary>
public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    /// <summary>
    ///     Finds all sensor readings that belong to an organization.
    /// </summary>
    Task<IEnumerable<SensorReading>> FindAllByOrganizationIdAsync(
        int organizationId,
        int? assetId = null,
        int? iotDeviceId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds all sensor readings that belong to an organization and IoT device.
    /// </summary>
    Task<IEnumerable<SensorReading>> FindAllByOrganizationIdAndIotDeviceIdAsync(
        int organizationId,
        int iotDeviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds one sensor reading by id and organization.
    /// </summary>
    Task<SensorReading?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);
}
