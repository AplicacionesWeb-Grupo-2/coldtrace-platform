namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when updating a maintenance schedule status.
/// </summary>
public enum UpdateMaintenanceScheduleStatusError
{
    OrganizationNotFound,
    MaintenanceScheduleNotFound,
    InvalidStatusTransition,
    UnexpectedError
}
