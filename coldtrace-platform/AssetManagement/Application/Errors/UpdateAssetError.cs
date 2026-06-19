namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while updating an asset.
/// </summary>
public enum UpdateAssetError
{
    /// <summary>
    ///     Another asset already uses the same UUID in the organization.
    /// </summary>
    DuplicateUuid,

    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The target asset was not found in the organization.
    /// </summary>
    AssetNotFound,

    /// <summary>
    ///     The placement location was not found in the organization.
    /// </summary>
    LocationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}