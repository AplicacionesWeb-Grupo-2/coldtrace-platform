using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing asset settings.
/// </summary>
[SwaggerSchema(Description = "An asset settings resource")]
public record AssetSettingsResource(
    int Id,
    int OrganizationId,
    int? AssetId,
    string Uuid,
    IReadOnlyList<string> AssetTypes,
    IReadOnlyList<string> IotDeviceTypes,
    double MinimumTemperature,
    double MaximumTemperature,
    double MinimumHumidity,
    double MaximumHumidity,
    int CalibrationFrequencyDays,
    string TemperatureUnit,
    string HumidityUnit,
    string WeightUnit,
    int ReadingFrequencySeconds,
    int AlertThresholdMinutes);
