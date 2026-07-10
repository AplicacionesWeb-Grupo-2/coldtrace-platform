using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a sensor reading resource from a domain entity.
/// </summary>
public static class SensorReadingResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a sensor reading entity into a response resource.
    /// </summary>
    public static SensorReadingResource ToResourceFromEntity(SensorReading entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.AssetId,
            entity.IotDeviceId,
            entity.GatewayId,
            entity.LocationId,
            entity.Temperature,
            entity.Humidity,
            entity.OutOfRange,
            entity.OutOfRange,
            entity.RecordedAt,
            entity.MotionDetected,
            entity.ImageCaptured,
            entity.BatteryLevel,
            entity.SignalStrength);
}
