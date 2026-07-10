namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while creating an organization.
/// </summary>
public enum CreateOrganizationError
{
    /// <summary>
    ///     An organization with the same contact email already exists.
    /// </summary>
    DuplicateContactEmail,

    /// <summary>
    ///     An organization with the same tax identifier already exists.
    /// </summary>
    DuplicateTaxId,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
