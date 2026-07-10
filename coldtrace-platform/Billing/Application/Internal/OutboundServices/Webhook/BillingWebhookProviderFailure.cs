namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;

/// <summary>
///     Provider-level webhook verification and parsing failures.
/// </summary>
public enum BillingWebhookProviderFailure
{
    NotConfigured,
    MissingSignature,
    InvalidSignature,
    InvalidPayload
}
