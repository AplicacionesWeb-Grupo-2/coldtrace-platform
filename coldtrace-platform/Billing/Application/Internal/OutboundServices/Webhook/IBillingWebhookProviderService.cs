using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;

/// <summary>
///     Outbound provider service for verifying and normalizing signed billing webhooks.
/// </summary>
public interface IBillingWebhookProviderService
{
    Task<Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>> ParseSignedEventAsync(
        string payload,
        string? signatureHeader,
        CancellationToken cancellationToken = default);
}
