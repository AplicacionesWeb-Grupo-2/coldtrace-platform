namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while querying users by organization.
/// </summary>
public enum GetUsersByOrganizationError
{
    /// <summary>
    ///     The organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
