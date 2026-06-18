namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving a location.
/// </summary>
public enum GetLocationByIdAndOrganizationError
{
    /// <summary>
    ///     The organization was not found.
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
