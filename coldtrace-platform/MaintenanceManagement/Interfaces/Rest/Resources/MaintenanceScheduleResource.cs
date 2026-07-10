namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST resource representing a maintenance schedule.
/// </summary>
public record MaintenanceScheduleResource(
    int Id,
    int OrganizationId,
    int AssetId,
    string Uuid,
    DateTimeOffset ScheduledDate,
    int? FrequencyDays,
    int? ResponsibleUserId,
    string? Observations,
    string Status,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
