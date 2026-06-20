namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while creating a gateway.
/// </summary>
public enum CreateGatewayError
{
    /// <summary>
    ///     A gateway with the same UUID already exists in the organization.
    /// </summary>
    DuplicateUuid,

    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The installation location was not found in the organization.
    /// </summary>
    LocationNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
