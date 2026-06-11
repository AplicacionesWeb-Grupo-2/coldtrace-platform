namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving locations that belong to one organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetLocationsByOrganizationIdQuery(int OrganizationId);
