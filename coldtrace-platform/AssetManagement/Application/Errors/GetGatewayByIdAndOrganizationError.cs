namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving a gateway.
/// </summary>
public enum GetGatewayByIdAndOrganizationError
{
    /// <summary>
    ///     The gateway was not found in the organization.
    /// </summary>
    GatewayNotFound,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
