using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a save asset settings command from a REST resource.
/// </summary>
public static class SaveAssetSettingsCommandFromResourceAssembler
{
    public static SaveAssetSettingsCommand ToCommandFromResource(
        SaveAssetSettingsResource resource,
        int organizationId,
        int? assetId) =>
        new(
            organizationId,
            assetId,
            resource.Uuid,
            resource.AssetTypes,
            resource.IotDeviceTypes,
            resource.MinimumTemperature,
            resource.MaximumTemperature,
            resource.MinimumHumidity,
            resource.MaximumHumidity,
            resource.CalibrationFrequencyDays,
            resource.TemperatureUnit,
            resource.HumidityUnit,
            resource.WeightUnit,
            resource.ReadingFrequencySeconds,
            resource.AlertThresholdMinutes);
}
