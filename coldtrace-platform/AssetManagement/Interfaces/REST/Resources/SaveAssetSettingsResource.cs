using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to save asset settings.
/// </summary>
[SwaggerSchema(Description = "Request payload to save asset settings")]
public record SaveAssetSettingsResource(
    [SwaggerParameter(Description = "Minimum safe temperature")]
    double TemperatureMin,
    [Required]
    [SwaggerParameter(Description = "Maximum safe temperature")]
    double TemperatureMax,
    [SwaggerParameter(Description = "Minimum safe humidity percentage")]
    double HumidityMin,
    [Required]
    [SwaggerParameter(Description = "Maximum safe humidity percentage")]
    double HumidityMax,
    [Required]
    [SwaggerParameter(Description = "Reading frequency in seconds")]
    int ReadingFrequencySeconds,
    [Required]
    [SwaggerParameter(Description = "Alert threshold in minutes")]
    int AlertThresholdMinutes);