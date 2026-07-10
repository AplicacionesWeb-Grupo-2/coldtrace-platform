using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

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
