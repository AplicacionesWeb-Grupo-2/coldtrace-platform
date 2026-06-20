namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for creating or updating organization default or asset-specific settings.
/// </summary>
public record SaveAssetSettingsCommand
{
    public SaveAssetSettingsCommand(
        int organizationId,
        int? assetId,
        string? uuid,
        IEnumerable<string>? assetTypes,
        IEnumerable<string>? iotDeviceTypes,
        double? minimumTemperature,
        double? maximumTemperature,
        double? minimumHumidity,
        double? maximumHumidity,
        int? calibrationFrequencyDays,
        string? temperatureUnit,
        string? humidityUnit,
        string? weightUnit,
        int? readingFrequencySeconds,
        int? alertThresholdMinutes)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositiveOrNull(assetId, nameof(assetId));
        Uuid = uuid?.Trim();
        AssetTypes = NormalizeList(assetTypes);
        IotDeviceTypes = NormalizeList(iotDeviceTypes);
        MinimumTemperature = RequireNumber(minimumTemperature, nameof(minimumTemperature));
        MaximumTemperature = RequireNumber(maximumTemperature, nameof(maximumTemperature));
        if (MinimumTemperature >= MaximumTemperature) throw new ArgumentException("Temperature range is invalid.");
        MinimumHumidity = minimumHumidity ?? 0;
        MaximumHumidity = RequireNumber(maximumHumidity, nameof(maximumHumidity));
        if (MinimumHumidity < 0 || MaximumHumidity > 100 || MinimumHumidity >= MaximumHumidity)
            throw new ArgumentException("Humidity range is invalid.");
        CalibrationFrequencyDays = RequirePositive(calibrationFrequencyDays, nameof(calibrationFrequencyDays));
        TemperatureUnit = RequireNonBlank(temperatureUnit);
        HumidityUnit = RequireNonBlank(humidityUnit);
        WeightUnit = RequireNonBlank(weightUnit);
        ReadingFrequencySeconds = readingFrequencySeconds is null
            ? 300
            : RequirePositive(readingFrequencySeconds, nameof(readingFrequencySeconds));
        AlertThresholdMinutes = alertThresholdMinutes is null
            ? 10
            : RequirePositive(alertThresholdMinutes, nameof(alertThresholdMinutes));
    }

    public int OrganizationId { get; init; }

    public int? AssetId { get; init; }

    public string? Uuid { get; init; }

    public IReadOnlyList<string> AssetTypes { get; init; }

    public IReadOnlyList<string> IotDeviceTypes { get; init; }

    public double MinimumTemperature { get; init; }

    public double MaximumTemperature { get; init; }

    public double MinimumHumidity { get; init; }

    public double MaximumHumidity { get; init; }

    public int CalibrationFrequencyDays { get; init; }

    public string TemperatureUnit { get; init; }

    public string HumidityUnit { get; init; }

    public string WeightUnit { get; init; }

    public int ReadingFrequencySeconds { get; init; }

    public int AlertThresholdMinutes { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int RequirePositive(int? value, string name)
    {
        if (value is null or <= 0) throw new ArgumentException($"{name} must be positive.");
        return value.Value;
    }

    private static int? RequirePositiveOrNull(int? value, string name)
    {
        if (value is <= 0) throw new ArgumentException($"{name} must be positive when provided.");
        return value;
    }

    private static double RequireNumber(double? value, string name)
    {
        if (value is null || double.IsNaN(value.Value) || double.IsInfinity(value.Value))
            throw new ArgumentException($"{name} is required.");
        return value.Value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }

    private static IReadOnlyList<string> NormalizeList(IEnumerable<string>? values) =>
        values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];
}
