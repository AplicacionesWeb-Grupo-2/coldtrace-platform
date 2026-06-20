namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for getting effective settings for an asset.
/// </summary>
public record GetEffectiveAssetSettingsByAssetIdQuery(int OrganizationId, int AssetId);
