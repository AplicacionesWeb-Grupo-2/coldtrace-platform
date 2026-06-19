namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one maintenance schedule by its identifier and owning organization.
/// </summary>
public record GetMaintenanceScheduleByIdAndOrganizationIdQuery(int OrganizationId, int MaintenanceScheduleId);
