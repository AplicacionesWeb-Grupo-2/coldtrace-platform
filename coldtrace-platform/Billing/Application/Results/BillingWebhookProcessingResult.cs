namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     Application result returned after processing one billing webhook event.
/// </summary>
public record BillingWebhookProcessingResult(
    string Provider,
    string EventId,
    string EventType,
    string ProcessingStatus,
    bool Duplicate,
    int? OrganizationId,
    string? PlanCode,
    string? SubscriptionStatus);
