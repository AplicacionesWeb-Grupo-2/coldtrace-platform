namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;

/// <summary>
///     Normalized provider webhook event used by the billing application service.
/// </summary>
public record BillingWebhookProviderEvent(
    string Provider,
    string EventId,
    string EventType,
    string? ObjectId,
    int? OrganizationId,
    string? ProviderCustomerId,
    string? ProviderSubscriptionId,
    string? TargetPlanCode,
    string? StripePriceId,
    string? SubscriptionStatus,
    DateTimeOffset? CurrentPeriodStart,
    DateTimeOffset? CurrentPeriodEnd,
    bool? CancelAtPeriodEnd,
    bool Supported);
