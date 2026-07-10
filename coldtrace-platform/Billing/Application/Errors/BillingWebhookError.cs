namespace ColdTrace.Platform.Billing.Application.Errors;

/// <summary>
///     Errors returned when processing signed billing provider webhooks.
/// </summary>
public enum BillingWebhookError
{
    ProviderNotConfigured,
    MissingSignature,
    InvalidSignature,
    InvalidPayload,
    ProcessingFailed
}
