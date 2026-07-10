using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to create a sensor reading.
/// </summary>
[SwaggerSchema(Description = "Request payload for recording backend-owned telemetry")]
public record CreateSensorReadingResource(
    [Required]
    [SwaggerParameter(Description = "Monitored asset identifier")]
    int? AssetId,
    [Required]
    [SwaggerParameter(Description = "IoT device identifier that produced the reading")]
    int? IotDeviceId,
    [SwaggerParameter(Description = "Temperature value")]
    double? Temperature,
    [SwaggerParameter(Description = "Humidity value")]
    double? Humidity,
    [SwaggerParameter(Description = "Reading timestamp; current time is used when omitted")]
    DateTimeOffset? RecordedAt,
    [SwaggerParameter(Description = "Whether motion was detected")]
    bool? MotionDetected,
    [SwaggerParameter(Description = "Whether an image was captured")]
    bool? ImageCaptured,
    [Range(0, 100)]
    [SwaggerParameter(Description = "Battery level percentage")]
    int? BatteryLevel,
    [Range(0, 100)]
    [SwaggerParameter(Description = "Signal strength percentage")]
    int? SignalStrength);
