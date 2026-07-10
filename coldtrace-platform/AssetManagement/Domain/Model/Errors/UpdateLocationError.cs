namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while updating a location.
/// </summary>
public enum UpdateLocationError
{
    /// <summary>
    ///     A location with the same name already exists in the organization.
    /// </summary>
    DuplicateName,

    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The location was not found in the organization.
    /// </summary>
    LocationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
