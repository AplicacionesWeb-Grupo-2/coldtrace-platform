using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing a sensor reading.
/// </summary>
[SwaggerSchema(Description = "A sensor reading resource")]
public record SensorReadingResource(
    [SwaggerParameter(Description = "Sensor reading identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "IoT device identifier")]
    int IotDeviceId,
    [SwaggerParameter(Description = "Reading metric name")]
    string Metric,
    [SwaggerParameter(Description = "Reading value")]
    decimal Value,
    [SwaggerParameter(Description = "Reading unit")]
    string Unit,
    [SwaggerParameter(Description = "Reading timestamp")]
    DateTimeOffset RecordedAt);
