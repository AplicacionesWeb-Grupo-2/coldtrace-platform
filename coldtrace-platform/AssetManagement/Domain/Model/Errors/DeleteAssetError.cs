namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while deleting an asset.
/// </summary>
public enum DeleteAssetError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The target asset was not found in the organization.
    /// </summary>
    AssetNotFound,

    /// <summary>
    ///     Related operational or historical records still depend on the asset.
    /// </summary>
    DeleteBlocked,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
