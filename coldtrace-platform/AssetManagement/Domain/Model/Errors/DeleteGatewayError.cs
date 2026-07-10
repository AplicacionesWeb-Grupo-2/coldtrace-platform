namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while deleting a gateway.
/// </summary>
public enum DeleteGatewayError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The gateway was not found in the organization.
    /// </summary>
    GatewayNotFound,

    /// <summary>
    ///     The gateway is still referenced by dependent records.
    /// </summary>
    DeleteBlocked,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
