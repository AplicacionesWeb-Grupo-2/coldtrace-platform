namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;

/// <summary>
///     Command for updating the lifecycle status of a maintenance schedule.
/// </summary>
public record UpdateMaintenanceScheduleStatusCommand
{
    private static readonly HashSet<string> SupportedStatuses =
        ["scheduled", "in_progress", "completed", "canceled"];

    /// <summary>
    ///     Creates a command with validated status data.
    /// </summary>
    public UpdateMaintenanceScheduleStatusCommand(int organizationId, int maintenanceScheduleId, string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        MaintenanceScheduleId = RequirePositive(maintenanceScheduleId, nameof(maintenanceScheduleId));
        Status = NormalizeStatus(status);
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the maintenance schedule identifier.</summary>
    public int MaintenanceScheduleId { get; init; }

    /// <summary>Gets the target lifecycle status.</summary>
    public string Status { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string NormalizeStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("status is required.");
        var normalized = value.Trim().ToLowerInvariant();
        if (!SupportedStatuses.Contains(normalized))
            throw new ArgumentException($"status '{normalized}' is not supported.");
        return normalized;
    }
}
