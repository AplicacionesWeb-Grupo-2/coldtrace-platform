namespace ColdTrace.Platform.Iam.Application.Results;

/// <summary>
///     Generic result returned after accepting a password reset request.
/// </summary>
/// <param name="Accepted">Whether the request was accepted.</param>
/// <param name="RequestedAt">Timestamp when the request was accepted.</param>
/// <param name="ExpiresAt">Timestamp when reset metadata expires.</param>
/// <param name="DeliveryStatus">Delivery state for the academic flow.</param>
public record PasswordResetRequestResult(
    bool Accepted,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    string DeliveryStatus);
