using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles asset settings resources from asset settings entities.
/// </summary>
public static class AssetSettingsResourceFromEntityAssembler
{
    public static AssetSettingsResource ToResourceFromEntity(AssetSettings entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.AssetId,
            entity.TemperatureMin,
            entity.TemperatureMax,
            entity.HumidityMin,
            entity.HumidityMax,
            entity.ReadingFrequencySeconds,
            entity.AlertThresholdMinutes);
}