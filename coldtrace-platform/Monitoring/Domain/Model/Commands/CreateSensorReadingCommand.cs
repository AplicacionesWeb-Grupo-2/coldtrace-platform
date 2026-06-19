namespace ColdTrace.Platform.Monitoring.Domain.Model.Commands;

/// <summary>
///     Command for creating a sensor reading.
/// </summary>
public record CreateSensorReadingCommand
{
    /// <summary>
    ///     Creates a validated sensor reading command.
    /// </summary>
    public CreateSensorReadingCommand(
        int organizationId,
        int iotDeviceId,
        string metric,
        decimal value,
        string unit,
        DateTimeOffset? recordedAt)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IotDeviceId = RequirePositive(iotDeviceId, nameof(iotDeviceId));
        Metric = RequireNonBlank(metric);
        Value = value;
        Unit = RequireNonBlank(unit);
        RecordedAt = recordedAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the originating IoT device identifier.
    /// </summary>
    public int IotDeviceId { get; init; }

    /// <summary>
    ///     Gets the metric name.
    /// </summary>
    public string Metric { get; init; }

    /// <summary>
    ///     Gets the measured value.
    /// </summary>
    public decimal Value { get; init; }

    /// <summary>
    ///     Gets the metric unit.
    /// </summary>
    public string Unit { get; init; }

    /// <summary>
    ///     Gets the timestamp when the reading was captured.
    /// </summary>
    public DateTimeOffset RecordedAt { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
