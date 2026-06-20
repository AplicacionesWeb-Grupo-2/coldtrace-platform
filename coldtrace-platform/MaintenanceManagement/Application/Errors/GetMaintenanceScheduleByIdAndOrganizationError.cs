namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when querying a maintenance schedule by id and organization.
/// </summary>
public enum GetMaintenanceScheduleByIdAndOrganizationError
{
    OrganizationNotFound,
    MaintenanceScheduleNotFound,
    UnexpectedError
}
