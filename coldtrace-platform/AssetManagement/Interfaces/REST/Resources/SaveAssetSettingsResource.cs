using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to create or update asset settings.
/// </summary>
[SwaggerSchema(Description = "Request payload for creating or updating safety thresholds and operational settings")]
public record SaveAssetSettingsResource(
    [SwaggerParameter(Description = "Optional settings code")]
    string? Uuid,
    [SwaggerParameter(Description = "Asset types covered by these settings")]
    IReadOnlyList<string>? AssetTypes,
    [SwaggerParameter(Description = "IoT device types covered by these settings")]
    IReadOnlyList<string>? IotDeviceTypes,
    [Required]
    [SwaggerParameter(Description = "Lower safe temperature threshold")]
    double? MinimumTemperature,
    [Required]
    [SwaggerParameter(Description = "Upper safe temperature threshold")]
    double? MaximumTemperature,
    [SwaggerParameter(Description = "Lower safe humidity threshold")]
    double? MinimumHumidity,
    [Required]
    [SwaggerParameter(Description = "Upper safe humidity threshold")]
    double? MaximumHumidity,
    [Required]
    [SwaggerParameter(Description = "Calibration frequency in days")]
    int? CalibrationFrequencyDays,
    [Required]
    [SwaggerParameter(Description = "Temperature unit")]
    string TemperatureUnit,
    [Required]
    [SwaggerParameter(Description = "Humidity unit")]
    string HumidityUnit,
    [Required]
    [SwaggerParameter(Description = "Weight unit")]
    string WeightUnit,
    [SwaggerParameter(Description = "Telemetry reading frequency in seconds")]
    int? ReadingFrequencySeconds,
    [SwaggerParameter(Description = "Delay threshold before alert escalation in minutes")]
    int? AlertThresholdMinutes);
