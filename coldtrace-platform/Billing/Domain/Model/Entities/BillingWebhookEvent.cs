using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Billing.Domain.Model.Entities;

/// <summary>
///     Stored provider webhook event used to keep Stripe processing idempotent.
/// </summary>
public class BillingWebhookEvent
{
    protected BillingWebhookEvent()
    {
        Provider = BillingProviders.Stripe;
        EventId = string.Empty;
        EventType = string.Empty;
        Status = BillingWebhookEventStatuses.Processed;
    }

    public BillingWebhookEvent(
        string? provider,
        string eventId,
        string eventType,
        string? status,
        int? organizationId,
        string? providerCustomerId,
        string? providerSubscriptionId,
        DateTimeOffset? processedAt,
        string? metadata)
    {
        Provider = NormalizeProvider(provider);
        EventId = RequireText(eventId, nameof(eventId));
        EventType = RequireText(eventType, nameof(eventType));
        Status = NormalizeStatus(status);
        OrganizationId = organizationId;
        ProviderCustomerId = NormalizeOptionalText(providerCustomerId);
        ProviderSubscriptionId = NormalizeOptionalText(providerSubscriptionId);
        ProcessedAt = processedAt ?? DateTimeOffset.UtcNow;
        Metadata = NormalizeOptionalText(metadata);
    }

    public int Id { get; private set; }

    public string Provider { get; private set; }

    public string EventId { get; private set; }

    public string EventType { get; private set; }

    public string Status { get; private set; }

    public int? OrganizationId { get; private set; }

    public string? ProviderCustomerId { get; private set; }

    public string? ProviderSubscriptionId { get; private set; }

    public DateTimeOffset ProcessedAt { get; private set; }

    public string? Metadata { get; private set; }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.", fieldName);

        return value.Trim();
    }

    private static string NormalizeProvider(string? value) =>
        string.IsNullOrWhiteSpace(value) ? BillingProviders.Stripe : value.Trim().ToUpperInvariant();

    private static string NormalizeStatus(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? BillingWebhookEventStatuses.Processed
            : value.Trim().ToUpperInvariant();

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
