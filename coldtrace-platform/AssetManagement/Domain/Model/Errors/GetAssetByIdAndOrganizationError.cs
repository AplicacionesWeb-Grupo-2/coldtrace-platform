namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving an asset.
/// </summary>
public enum GetAssetByIdAndOrganizationError
{
    /// <summary>
    ///     The organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The asset was not found in the organization.
    /// </summary>
    AssetNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}