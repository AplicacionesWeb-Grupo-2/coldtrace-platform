namespace ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Stable processing status values for signed billing provider webhook events.
/// </summary>
public static class BillingWebhookEventStatuses
{
    public const string Processed = "PROCESSED";
    public const string Ignored = "IGNORED";
}
