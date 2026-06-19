using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing asset settings.
/// </summary>
[SwaggerSchema(Description = "An asset settings resource")]
public record AssetSettingsResource(
    [SwaggerParameter(Description = "Asset settings identifier")] int Id,
    [SwaggerParameter(Description = "Organization identifier")] int OrganizationId,
    [SwaggerParameter(Description = "Asset identifier. Null means organization default settings")] int? AssetId,
    [SwaggerParameter(Description = "Minimum safe temperature")] double TemperatureMin,
    [SwaggerParameter(Description = "Maximum safe temperature")] double TemperatureMax,
    [SwaggerParameter(Description = "Minimum safe humidity percentage")] double HumidityMin,
    [SwaggerParameter(Description = "Maximum safe humidity percentage")] double HumidityMax,
    [SwaggerParameter(Description = "Reading frequency in seconds")] int ReadingFrequencySeconds,
    [SwaggerParameter(Description = "Alert threshold in minutes")] int AlertThresholdMinutes);