using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a sensor reading command from a REST resource.
/// </summary>
public static class CreateSensorReadingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create sensor reading request into a command.
    /// </summary>
    public static CreateSensorReadingCommand ToCommandFromResource(
        CreateSensorReadingResource resource,
        int organizationId) =>
        new(
            organizationId,
            resource.AssetId,
            resource.IotDeviceId,
            resource.Temperature,
            resource.Humidity,
            resource.RecordedAt,
            resource.MotionDetected,
            resource.ImageCaptured,
            resource.BatteryLevel,
            resource.SignalStrength);
}
