namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for manually registering an organization incident.
/// </summary>
public record CreateIncidentCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized incident data.
    /// </summary>
    public CreateIncidentCommand(
        int organizationId,
        int? assetId,
        int? deviceId,
        int? readingId,
        string? assetName,
        string? deviceName,
        string type,
        string severity,
        string? value)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositiveWhenPresent(assetId, nameof(assetId));
        DeviceId = RequirePositiveWhenPresent(deviceId, nameof(deviceId));
        ReadingId = RequirePositiveWhenPresent(readingId, nameof(readingId));
        AssetName = NormalizeOptionalText(assetName);
        DeviceName = NormalizeOptionalText(deviceName);
        Type = RequireNonBlank(type);
        Severity = NormalizeSeverity(severity);
        Value = NormalizeOptionalText(value);
    }

    public int OrganizationId { get; init; }

    public int? AssetId { get; init; }

    public int? DeviceId { get; init; }

    public int? ReadingId { get; init; }

    public string? AssetName { get; init; }

    public string? DeviceName { get; init; }

    public string Type { get; init; }

    public string Severity { get; init; }

    public string? Value { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int? RequirePositiveWhenPresent(int? value, string name)
    {
        if (value is null) return null;
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }

    private static string NormalizeSeverity(string? value)
    {
        var severity = RequireNonBlank(value).ToLowerInvariant();
        if (severity is not ("warning" or "critical"))
            throw new ArgumentException("Incident severity must be warning or critical.");
        return severity;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
