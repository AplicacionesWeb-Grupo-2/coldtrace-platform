namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when creating a maintenance schedule.
/// </summary>
public enum CreateMaintenanceScheduleError
{
    OrganizationNotFound,
    AssetNotFound,
    DuplicateActiveSchedule,
    UnexpectedError
}
