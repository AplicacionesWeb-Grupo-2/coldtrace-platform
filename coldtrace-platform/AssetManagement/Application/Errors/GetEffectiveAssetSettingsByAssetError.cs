namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving effective asset settings for an asset.
/// </summary>
public enum GetEffectiveAssetSettingsByAssetError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The asset was not found or does not belong to the organization.
    /// </summary>
    AssetNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}