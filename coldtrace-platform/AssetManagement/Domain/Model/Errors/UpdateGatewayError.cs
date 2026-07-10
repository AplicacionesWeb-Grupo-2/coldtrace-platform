namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while updating a gateway.
/// </summary>
public enum UpdateGatewayError
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
    ///     The gateway was not found in the organization.
    /// </summary>
    GatewayNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
