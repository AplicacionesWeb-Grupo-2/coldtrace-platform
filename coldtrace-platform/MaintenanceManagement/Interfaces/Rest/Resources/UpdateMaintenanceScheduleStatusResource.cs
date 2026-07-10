namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST resource for updating the lifecycle status of a maintenance schedule.
/// </summary>
public record UpdateMaintenanceScheduleStatusResource(string Status);
