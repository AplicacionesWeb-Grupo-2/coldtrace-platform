namespace ColdTrace.Platform.Monitoring.Domain.Model.Commands;

/// <summary>
///     Command for creating a sensor reading explicitly.
/// </summary>
public record CreateSensorReadingCommand
{
    public CreateSensorReadingCommand(
        int organizationId,
        int? assetId,
        int? iotDeviceId,
        double? temperature,
        double? humidity,
        DateTimeOffset? recordedAt,
        bool? motionDetected,
        bool? imageCaptured,
        int? batteryLevel,
        int? signalStrength)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositive(assetId, nameof(assetId));
        IotDeviceId = RequirePositive(iotDeviceId, nameof(iotDeviceId));
        Temperature = temperature;
        Humidity = humidity;
        RecordedAt = recordedAt ?? DateTimeOffset.UtcNow;
        MotionDetected = motionDetected;
        ImageCaptured = imageCaptured;
        BatteryLevel = RequirePercentageOrNull(batteryLevel, nameof(batteryLevel));
        SignalStrength = RequirePercentageOrNull(signalStrength, nameof(signalStrength));
        if (Temperature is null && Humidity is null && MotionDetected is null && ImageCaptured is null &&
            BatteryLevel is null && SignalStrength is null)
            throw new ArgumentException("At least one telemetry value is required.");
    }

    public int OrganizationId { get; init; }

    public int AssetId { get; init; }

    public int IotDeviceId { get; init; }

    public double? Temperature { get; init; }

    public double? Humidity { get; init; }

    public DateTimeOffset RecordedAt { get; init; }

    public bool? MotionDetected { get; init; }

    public bool? ImageCaptured { get; init; }

    public int? BatteryLevel { get; init; }

    public int? SignalStrength { get; init; }

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

    private static int? RequirePercentageOrNull(int? value, string name)
    {
        if (value is < 0 or > 100) throw new ArgumentException($"{name} must be between 0 and 100.");
        return value;
    }
}
