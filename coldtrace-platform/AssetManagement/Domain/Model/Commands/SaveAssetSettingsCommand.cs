namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for saving asset settings for an organization asset or as organization default.
/// </summary>
public record SaveAssetSettingsCommand
{
    /// <summary>
    ///     Creates a command with validated asset settings data.
    /// </summary>
    public SaveAssetSettingsCommand(
        int organizationId,
        int? assetId,
        double temperatureMin,
        double temperatureMax,
        double humidityMin,
        double humidityMax,
        int readingFrequencySeconds,
        int alertThresholdMinutes)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = assetId.HasValue ? RequirePositive(assetId.Value, nameof(assetId)) : null;
        TemperatureMin = temperatureMin;
        TemperatureMax = RequireGreaterThan(temperatureMax, temperatureMin, nameof(temperatureMax));
        HumidityMin = RequireInRange(humidityMin, 0, 100, nameof(humidityMin));
        HumidityMax = RequireInRange(humidityMax, humidityMin, 100, nameof(humidityMax));
        ReadingFrequencySeconds = RequirePositive(readingFrequencySeconds, nameof(readingFrequencySeconds));
        AlertThresholdMinutes = RequirePositive(alertThresholdMinutes, nameof(alertThresholdMinutes));
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the optional asset identifier. Null means organization default settings.</summary>
    public int? AssetId { get; init; }

    /// <summary>Gets the minimum safe temperature.</summary>
    public double TemperatureMin { get; init; }

    /// <summary>Gets the maximum safe temperature.</summary>
    public double TemperatureMax { get; init; }

    /// <summary>Gets the minimum safe humidity percentage.</summary>
    public double HumidityMin { get; init; }

    /// <summary>Gets the maximum safe humidity percentage.</summary>
    public double HumidityMax { get; init; }

    /// <summary>Gets the reading frequency in seconds.</summary>
    public int ReadingFrequencySeconds { get; init; }

    /// <summary>Gets the alert threshold in minutes.</summary>
    public int AlertThresholdMinutes { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static double RequireGreaterThan(double value, double min, string name)
    {
        if (value <= min) throw new ArgumentException($"{name} must be greater than {min}.");
        return value;
    }

    private static double RequireInRange(double value, double min, double max, string name)
    {
        if (value < min || value > max) throw new ArgumentException($"{name} must be between {min} and {max}.");
        return value;
    }
}