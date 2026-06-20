using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Asset settings aggregate for safety thresholds and telemetry defaults.
/// </summary>
public class AssetSettings : IAuditableEntity
{
    protected AssetSettings()
    {
        Uuid = string.Empty;
        TemperatureUnit = string.Empty;
        HumidityUnit = string.Empty;
        WeightUnit = string.Empty;
    }

    /// <summary>
    ///     Creates asset settings from a save command.
    /// </summary>
    public AssetSettings(SaveAssetSettingsCommand command)
    {
        TemperatureUnit = string.Empty;
        HumidityUnit = string.Empty;
        WeightUnit = string.Empty;
        OrganizationId = command.OrganizationId;
        AssetId = command.AssetId;
        Uuid = string.IsNullOrWhiteSpace(command.Uuid)
            ? $"AST-SET-{Guid.NewGuid()}"
            : command.Uuid.Trim();
        Apply(command);
    }

    public int Id { get; private set; }

    public int OrganizationId { get; private set; }

    public int? AssetId { get; private set; }

    public string Uuid { get; private set; }

    public List<AssetSettingsAssetType> AssetTypeEntries { get; private set; } = [];

    public List<AssetSettingsIotDeviceType> IotDeviceTypeEntries { get; private set; } = [];

    public IReadOnlyList<string> AssetTypes => AssetTypeEntries.Select(entry => entry.AssetType).ToList();

    public IReadOnlyList<string> IotDeviceTypes => IotDeviceTypeEntries.Select(entry => entry.IotDeviceType).ToList();

    public double MinimumTemperature { get; private set; }

    public double MaximumTemperature { get; private set; }

    public double MinimumHumidity { get; private set; }

    public double MaximumHumidity { get; private set; }

    public int CalibrationFrequencyDays { get; private set; }

    public string TemperatureUnit { get; private set; }

    public string HumidityUnit { get; private set; }

    public string WeightUnit { get; private set; }

    public int ReadingFrequencySeconds { get; private set; }

    public int AlertThresholdMinutes { get; private set; }

    public Organization Organization { get; private set; } = null!;

    public Asset? Asset { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates threshold and operational values from a save command.
    /// </summary>
    public void Update(SaveAssetSettingsCommand command)
    {
        if (!string.IsNullOrWhiteSpace(command.Uuid)) Uuid = command.Uuid.Trim();
        Apply(command);
    }

    private void Apply(SaveAssetSettingsCommand command)
    {
        AssetTypeEntries = command.AssetTypes
            .Select(assetType => new AssetSettingsAssetType(assetType))
            .ToList();
        IotDeviceTypeEntries = command.IotDeviceTypes
            .Select(iotDeviceType => new AssetSettingsIotDeviceType(iotDeviceType))
            .ToList();
        MinimumTemperature = command.MinimumTemperature;
        MaximumTemperature = command.MaximumTemperature;
        MinimumHumidity = command.MinimumHumidity;
        MaximumHumidity = command.MaximumHumidity;
        CalibrationFrequencyDays = command.CalibrationFrequencyDays;
        TemperatureUnit = command.TemperatureUnit;
        HumidityUnit = command.HumidityUnit;
        WeightUnit = command.WeightUnit;
        ReadingFrequencySeconds = command.ReadingFrequencySeconds;
        AlertThresholdMinutes = command.AlertThresholdMinutes;
    }
}
