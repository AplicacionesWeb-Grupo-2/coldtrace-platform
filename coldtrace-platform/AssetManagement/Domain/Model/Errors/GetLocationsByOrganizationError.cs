namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving organization locations.
/// </summary>
public enum GetLocationsByOrganizationError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
