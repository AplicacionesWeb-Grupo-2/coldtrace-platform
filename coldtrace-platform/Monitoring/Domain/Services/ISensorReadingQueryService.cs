using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Monitoring.Domain.Services;

/// <summary>
///     Sensor reading query service contract.
/// </summary>
public interface ISensorReadingQueryService
{
    /// <summary>
    ///     Handles a query for all sensor readings in an organization.
    /// </summary>
    Task<Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>> Handle(
        GetSensorReadingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles a query for sensor readings of one IoT device in an organization.
    /// </summary>
    Task<Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>> Handle(
        GetSensorReadingsByIotDeviceAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles a query for one sensor reading in an organization.
    /// </summary>
    Task<Result<SensorReading, GetSensorReadingByIdAndOrganizationError>> Handle(
        GetSensorReadingByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
