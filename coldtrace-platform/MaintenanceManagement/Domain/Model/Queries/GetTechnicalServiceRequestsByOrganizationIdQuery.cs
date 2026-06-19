namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all technical service requests for an organization.
/// </summary>
public record GetTechnicalServiceRequestsByOrganizationIdQuery(int OrganizationId);
