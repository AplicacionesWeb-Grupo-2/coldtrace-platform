using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;

/// <summary>
///     Outbound service for provider-hosted checkout sessions.
/// </summary>
public interface ICheckoutSessionProviderService
{
    Task<Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>> CreateSubscriptionCheckoutSessionAsync(
        CheckoutSessionProviderRequest request,
        CancellationToken cancellationToken = default);
}
