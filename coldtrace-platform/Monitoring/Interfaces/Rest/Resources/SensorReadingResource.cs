using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a sensor reading.
/// </summary>
[SwaggerSchema(Description = "A sensor reading resource")]
public record SensorReadingResource(
    int Id,
    int OrganizationId,
    int AssetId,
    int IotDeviceId,
    int GatewayId,
    int LocationId,
    double? Temperature,
    double? Humidity,
    bool OutOfRange,
    bool IsOutOfRange,
    DateTimeOffset RecordedAt,
    bool? MotionDetected,
    bool? ImageCaptured,
    int? BatteryLevel,
    int? SignalStrength);
