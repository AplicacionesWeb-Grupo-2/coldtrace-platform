namespace ColdTrace.Platform.IdentityAccess.Application.Errors;

/// <summary>
///     Errors that can occur while creating a user.
/// </summary>
public enum CreateUserError
{
    /// <summary>
    ///     A user with the same email already exists.
    /// </summary>
    DuplicateEmail,

    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The assigned role was not found.
    /// </summary>
    RoleNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
