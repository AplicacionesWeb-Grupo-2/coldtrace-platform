namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;

/// <summary>
///     Query to retrieve one technical service request by id within an organization.
/// </summary>
public record GetTechnicalServiceRequestByIdAndOrganizationIdQuery(
    int OrganizationId,
    int TechnicalServiceRequestId);
