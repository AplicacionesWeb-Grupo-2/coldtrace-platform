using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Generic acceptance response for a password reset request.
/// </summary>
public record PasswordResetRequestResource(
    [property: SwaggerSchema(Description = "Whether the request was accepted")]
    bool Accepted,
    [property: SwaggerSchema(Description = "Timestamp when the request was accepted")]
    DateTimeOffset RequestedAt,
    [property: SwaggerSchema(Description = "Timestamp when reset metadata expires")]
    DateTimeOffset ExpiresAt,
    [property: SwaggerSchema(Description = "Delivery state for the reset email integration")]
    string DeliveryStatus);
