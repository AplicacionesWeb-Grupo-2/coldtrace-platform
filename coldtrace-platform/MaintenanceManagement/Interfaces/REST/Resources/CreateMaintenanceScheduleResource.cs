namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

/// <summary>
///     REST resource for creating a maintenance schedule.
/// </summary>
public record CreateMaintenanceScheduleResource(
    int AssetId,
    DateTimeOffset ScheduledDate,
    int? FrequencyDays,
    int? ResponsibleUserId,
    string? Observations,
    string Status);
