namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving all maintenance schedules owned by an organization.
/// </summary>
public record GetMaintenanceSchedulesByOrganizationIdQuery(int OrganizationId);
