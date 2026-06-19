namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving the effective asset settings for a specific asset.
///     Returns asset-specific settings if they exist, otherwise the organization default.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="AssetId">Asset identifier.</param>
public record GetEffectiveAssetSettingsByAssetIdQuery(int OrganizationId, int AssetId);