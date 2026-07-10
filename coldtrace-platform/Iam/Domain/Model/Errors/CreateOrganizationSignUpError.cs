namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while signing up an organization.
/// </summary>
public enum CreateOrganizationSignUpError
{
    /// <summary>
    ///     An organization with the same contact email already exists.
    /// </summary>
    DuplicateOrganizationContactEmail,

    /// <summary>
    ///     An organization with the same tax identifier already exists.
    /// </summary>
    DuplicateOrganizationTaxId,

    /// <summary>
    ///     A user with the same email already exists.
    /// </summary>
    DuplicateUserEmail,

    /// <summary>
    ///     The initial role required for sign-up is missing.
    /// </summary>
    InitialRoleNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
