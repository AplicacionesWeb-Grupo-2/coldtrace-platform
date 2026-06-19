using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a SaveAssetSettingsCommand from a SaveAssetSettingsResource.
/// </summary>
public static class SaveAssetSettingsCommandFromResourceAssembler
{
    public static SaveAssetSettingsCommand ToCommandFromResource(
        SaveAssetSettingsResource resource,
        int organizationId,
        int? assetId = null) =>
        new(organizationId, assetId, resource.TemperatureMin, resource.TemperatureMax,
            resource.HumidityMin, resource.HumidityMax,
            resource.ReadingFrequencySeconds, resource.AlertThresholdMinutes);
}