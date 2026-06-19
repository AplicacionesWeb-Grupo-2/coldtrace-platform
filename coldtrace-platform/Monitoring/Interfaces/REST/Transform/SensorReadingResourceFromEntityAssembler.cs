using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

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
            entity.IotDeviceId,
            entity.Metric,
            entity.Value,
            entity.Unit,
            entity.RecordedAt);
}
