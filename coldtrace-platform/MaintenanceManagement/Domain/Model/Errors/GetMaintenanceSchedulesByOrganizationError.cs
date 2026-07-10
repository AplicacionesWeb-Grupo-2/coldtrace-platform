namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur when querying maintenance schedules by organization.
/// </summary>
public enum GetMaintenanceSchedulesByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
