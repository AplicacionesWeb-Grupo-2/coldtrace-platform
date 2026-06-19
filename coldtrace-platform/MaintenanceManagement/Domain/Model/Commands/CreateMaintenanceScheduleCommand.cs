namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;

/// <summary>
///     Command for creating a preventive maintenance schedule.
/// </summary>
public record CreateMaintenanceScheduleCommand
{
    private static readonly HashSet<string> SupportedStatuses =
        ["scheduled", "in_progress", "completed", "canceled"];

    /// <summary>
    ///     Creates a command with validated and normalized maintenance schedule data.
    /// </summary>
    public CreateMaintenanceScheduleCommand(
        int organizationId,
        int assetId,
        DateTimeOffset scheduledDate,
        int? frequencyDays,
        int? responsibleUserId,
        string? observations,
        string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositive(assetId, nameof(assetId));
        ScheduledDate = RequireFutureDate(scheduledDate);
        FrequencyDays = RequirePositiveWhenPresent(frequencyDays, nameof(frequencyDays));
        ResponsibleUserId = RequirePositiveWhenPresent(responsibleUserId, nameof(responsibleUserId));
        Observations = observations?.Trim();
        Status = NormalizeStatus(status);
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the maintained asset identifier.</summary>
    public int AssetId { get; init; }

    /// <summary>Gets the planned maintenance date.</summary>
    public DateTimeOffset ScheduledDate { get; init; }

    /// <summary>Gets the optional recurrence cadence in days.</summary>
    public int? FrequencyDays { get; init; }

    /// <summary>Gets the optional responsible user identifier.</summary>
    public int? ResponsibleUserId { get; init; }

    /// <summary>Gets the optional planning notes.</summary>
    public string? Observations { get; init; }

    /// <summary>Gets the initial lifecycle status.</summary>
    public string Status { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int? RequirePositiveWhenPresent(int? value, string name)
    {
        if (value.HasValue && value.Value <= 0)
            throw new ArgumentException($"{name} must be positive when provided.");
        return value;
    }

    private static DateTimeOffset RequireFutureDate(DateTimeOffset value)
    {
        if (value < DateTimeOffset.UtcNow.AddMinutes(-1))
            throw new ArgumentException("scheduledDate must be a present or future date.");
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
