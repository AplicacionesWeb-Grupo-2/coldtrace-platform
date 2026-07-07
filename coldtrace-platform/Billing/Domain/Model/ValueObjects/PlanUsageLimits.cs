namespace ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Usage limits exposed by a subscription plan.
/// </summary>
public record PlanUsageLimits
{
    public PlanUsageLimits(
        int? maxLocations,
        int? maxAssets,
        int? maxIotDevices,
        int? maxUsers,
        int? historyRetentionDays)
    {
        MaxLocations = RequirePositiveOrNull(maxLocations, nameof(maxLocations));
        MaxAssets = RequirePositiveOrNull(maxAssets, nameof(maxAssets));
        MaxIotDevices = RequirePositiveOrNull(maxIotDevices, nameof(maxIotDevices));
        MaxUsers = RequirePositiveOrNull(maxUsers, nameof(maxUsers));
        HistoryRetentionDays = RequirePositiveOrNull(historyRetentionDays, nameof(historyRetentionDays));
    }

    public int? MaxLocations { get; }

    public int? MaxAssets { get; }

    public int? MaxIotDevices { get; }

    public int? MaxUsers { get; }

    public int? HistoryRetentionDays { get; }

    private static int? RequirePositiveOrNull(int? value, string fieldName)
    {
        if (value is <= 0)
            throw new ArgumentException($"{fieldName} must be positive when provided.", fieldName);

        return value;
    }
}
