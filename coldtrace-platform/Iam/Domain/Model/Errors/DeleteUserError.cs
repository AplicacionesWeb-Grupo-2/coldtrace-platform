namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while deleting an organization user.
/// </summary>
public enum DeleteUserError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The user was not found in the organization.
    /// </summary>
    UserNotFound,

    /// <summary>
    ///     Related records prevent the user from being deleted.
    /// </summary>
    DeleteBlocked,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
