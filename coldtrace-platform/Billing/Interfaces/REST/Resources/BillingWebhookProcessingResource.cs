using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     REST resource returned after processing a billing webhook.
/// </summary>
[SwaggerSchema(Description = "Billing webhook processing result")]
public record BillingWebhookProcessingResource(
    [SwaggerParameter(Description = "Billing provider")]
    string Provider,
    [SwaggerParameter(Description = "Provider event identifier")]
    string EventId,
    [SwaggerParameter(Description = "Provider event type")]
    string EventType,
    [SwaggerParameter(Description = "Local processing status")]
    string ProcessingStatus,
    [SwaggerParameter(Description = "Whether this event had already been processed")]
    bool Duplicate,
    [SwaggerParameter(Description = "Updated organization identifier when available")]
    int? OrganizationId,
    [SwaggerParameter(Description = "Resulting local plan code when updated")]
    string? PlanCode,
    [SwaggerParameter(Description = "Resulting local subscription status when updated")]
    string? SubscriptionStatus);
