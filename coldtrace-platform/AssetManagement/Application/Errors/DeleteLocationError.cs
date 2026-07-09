namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while deleting a location.
/// </summary>
public enum DeleteLocationError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The location was not found in the organization.
    /// </summary>
    LocationNotFound,

    /// <summary>
    ///     Assets, gateways, or another related record still depend on the location.
    /// </summary>
    DeleteBlocked,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
