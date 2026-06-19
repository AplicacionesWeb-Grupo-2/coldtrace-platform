namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving assets that belong to one organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetAssetsByOrganizationIdQuery(int OrganizationId);