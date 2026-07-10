using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Converts password reset request results to REST resources.
/// </summary>
public static class PasswordResetRequestResourceFromResultAssembler
{
    /// <summary>
    ///     Converts the application result to the external response contract.
    /// </summary>
    public static PasswordResetRequestResource ToResourceFromResult(PasswordResetRequestResult result) =>
        new(result.Accepted, result.RequestedAt, result.ExpiresAt, result.DeliveryStatus);
}
