namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one asset by id within an organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="AssetId">Asset identifier.</param>
public record GetAssetByIdAndOrganizationIdQuery(int OrganizationId, int AssetId);