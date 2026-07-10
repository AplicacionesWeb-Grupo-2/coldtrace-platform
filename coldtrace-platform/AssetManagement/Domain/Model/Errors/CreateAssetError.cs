namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while creating an asset.
/// </summary>
public enum CreateAssetError
{
    /// <summary>
    ///     An asset with the same UUID already exists in the organization.
    /// </summary>
    DuplicateUuid,

    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The placement location was not found in the organization.
    /// </summary>
    LocationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}