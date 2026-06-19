using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

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
        int organizationId,
        int iotDeviceId) =>
        new(
            organizationId,
            iotDeviceId,
            resource.Metric,
            resource.Value,
            resource.Unit,
            resource.RecordedAt);
}
