namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving IoT devices by organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetIotDevicesByOrganizationIdQuery(int OrganizationId);
