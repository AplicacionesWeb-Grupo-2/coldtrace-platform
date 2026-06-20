namespace ColdTrace.Platform.Monitoring.Domain.Model.Commands;

/// <summary>
///     Command for generating backend-owned demo sensor readings.
/// </summary>
public record GenerateDemoSensorReadingsCommand
{
    public GenerateDemoSensorReadingsCommand(int organizationId, int? assetId, int? count)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositiveOrNull(assetId, nameof(assetId));
        Count = count ?? 1;
        if (Count is <= 0 or > 50) throw new ArgumentException("count must be between 1 and 50.");
    }

    public int OrganizationId { get; init; }

    public int? AssetId { get; init; }

    public int Count { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int? RequirePositiveOrNull(int? value, string name)
    {
        if (value is <= 0) throw new ArgumentException($"{name} must be positive when provided.");
        return value;
    }
}
