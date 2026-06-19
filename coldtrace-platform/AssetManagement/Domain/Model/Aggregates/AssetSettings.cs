using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     AssetSettings aggregate for the asset management context.
/// </summary>
/// <remarks>
///     Represents safety ranges and operational thresholds tied to an organization
///     and optionally to a specific asset. When AssetId is null, these are the
///     organization-wide default settings.
/// </remarks>
public class AssetSettings : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected AssetSettings() { }

    /// <summary>
    ///     Creates asset settings from a save command.
    /// </summary>
    /// <param name="command">Command containing asset settings data.</param>
    public AssetSettings(SaveAssetSettingsCommand command)
    {
        OrganizationId = command.OrganizationId;
        AssetId = command.AssetId;
        TemperatureMin = command.TemperatureMin;
        TemperatureMax = command.TemperatureMax;
        HumidityMin = command.HumidityMin;
        HumidityMax = command.HumidityMax;
        ReadingFrequencySeconds = command.ReadingFrequencySeconds;
        AlertThresholdMinutes = command.AlertThresholdMinutes;
    }

    /// <summary>Gets the server-generated identifier.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; private set; }

    /// <summary>Gets the optional asset identifier. Null means organization default settings.</summary>
    public int? AssetId { get; private set; }

    /// <summary>Gets the owning organization.</summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>Gets the optional asset these settings apply to.</summary>
    public Asset? Asset { get; private set; }

    /// <summary>Gets the minimum safe temperature.</summary>
    public double TemperatureMin { get; private set; }

    /// <summary>Gets the maximum safe temperature.</summary>
    public double TemperatureMax { get; private set; }

    /// <summary>Gets the minimum safe humidity percentage.</summary>
    public double HumidityMin { get; private set; }

    /// <summary>Gets the maximum safe humidity percentage.</summary>
    public double HumidityMax { get; private set; }

    /// <summary>Gets the reading frequency in seconds.</summary>
    public int ReadingFrequencySeconds { get; private set; }

    /// <summary>Gets the alert threshold in minutes.</summary>
    public int AlertThresholdMinutes { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates asset settings data.
    /// </summary>
    /// <param name="command">Command containing updated data.</param>
    public void Update(SaveAssetSettingsCommand command)
    {
        TemperatureMin = command.TemperatureMin;
        TemperatureMax = command.TemperatureMax;
        HumidityMin = command.HumidityMin;
        HumidityMax = command.HumidityMax;
        ReadingFrequencySeconds = command.ReadingFrequencySeconds;
        AlertThresholdMinutes = command.AlertThresholdMinutes;
    }
}