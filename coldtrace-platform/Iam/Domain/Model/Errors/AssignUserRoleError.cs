namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while assigning a role to a user.
/// </summary>
public enum AssignUserRoleError
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
    ///     The assigned role was not found.
    /// </summary>
    RoleNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
