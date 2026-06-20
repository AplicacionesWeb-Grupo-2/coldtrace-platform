using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an asset settings resource from a domain entity.
/// </summary>
public static class AssetSettingsResourceFromEntityAssembler
{
    public static AssetSettingsResource ToResourceFromEntity(AssetSettings entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.AssetId,
            entity.Uuid,
            entity.AssetTypes,
            entity.IotDeviceTypes,
            entity.MinimumTemperature,
            entity.MaximumTemperature,
            entity.MinimumHumidity,
            entity.MaximumHumidity,
            entity.CalibrationFrequencyDays,
            entity.TemperatureUnit,
            entity.HumidityUnit,
            entity.WeightUnit,
            entity.ReadingFrequencySeconds,
            entity.AlertThresholdMinutes);
}
