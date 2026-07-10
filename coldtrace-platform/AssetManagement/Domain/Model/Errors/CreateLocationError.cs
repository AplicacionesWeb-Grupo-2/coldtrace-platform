namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while creating a location.
/// </summary>
public enum CreateLocationError
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
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
