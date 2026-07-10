using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;

/// <summary>
///     Preventive maintenance schedule aggregate.
/// </summary>
public class MaintenanceSchedule : IAuditableEntity
{
    private static readonly HashSet<string> TerminalStatuses = ["completed", "canceled"];

    private static readonly HashSet<string> ValidStatuses =
        ["scheduled", "in_progress", "completed", "canceled"];

    protected MaintenanceSchedule()
    {
        Uuid = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates a maintenance schedule from a create command.
    /// </summary>
    public MaintenanceSchedule(CreateMaintenanceScheduleCommand command)
    {
        OrganizationId = command.OrganizationId;
        AssetId = command.AssetId;
        Uuid = "MNT-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        ScheduledDate = command.ScheduledDate;
        FrequencyDays = command.FrequencyDays;
        ResponsibleUserId = command.ResponsibleUserId;
        Observations = command.Observations;
        Status = command.Status;
    }

    /// <summary>Gets the server-generated identifier.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; private set; }

    /// <summary>Gets the maintained asset identifier.</summary>
    public int AssetId { get; private set; }

    /// <summary>Gets the auto-generated public schedule code.</summary>
    public string Uuid { get; private set; }

    /// <summary>Gets the planned maintenance date.</summary>
    public DateTimeOffset ScheduledDate { get; private set; }

    /// <summary>Gets the optional recurrence cadence in days.</summary>
    public int? FrequencyDays { get; private set; }

    /// <summary>Gets the optional responsible user identifier.</summary>
    public int? ResponsibleUserId { get; private set; }

    /// <summary>Gets the optional planning notes.</summary>
    public string? Observations { get; private set; }

    /// <summary>Gets the lifecycle status.</summary>
    public string Status { get; private set; }

    /// <summary>Gets the maintained asset.</summary>
    public Asset Asset { get; private set; } = null!;

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Returns true when the schedule is not in a terminal status.
    /// </summary>
    public bool IsActive() => !TerminalStatuses.Contains(Status);

    /// <summary>
    ///     Returns true when the current status can transition to the requested status.
    /// </summary>
    public bool CanTransitionTo(string requestedStatus)
    {
        if (!ValidStatuses.Contains(requestedStatus)) return false;
        if (Status == requestedStatus) return true;
        if (TerminalStatuses.Contains(Status)) return false;
        return Status switch
        {
            "scheduled" => requestedStatus is "in_progress" or "completed" or "canceled",
            "in_progress" => requestedStatus is "completed" or "canceled",
            _ => false
        };
    }

    /// <summary>
    ///     Updates the lifecycle status.
    /// </summary>
    public void UpdateStatus(string status) => Status = status;
}
