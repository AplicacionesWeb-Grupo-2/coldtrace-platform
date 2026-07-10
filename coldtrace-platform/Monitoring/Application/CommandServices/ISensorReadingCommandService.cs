using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Monitoring.Application.CommandServices;

/// <summary>
///     Sensor reading command service contract.
/// </summary>
public interface ISensorReadingCommandService
{
    /// <summary>
    ///     Handles a sensor reading creation command.
    /// </summary>
    Task<Result<SensorReading, CreateSensorReadingError>> Handle(
        CreateSensorReadingCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles demo sensor reading generation.
    /// </summary>
    Task<Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>> Handle(
        GenerateDemoSensorReadingsCommand command,
        CancellationToken cancellationToken = default);
}
