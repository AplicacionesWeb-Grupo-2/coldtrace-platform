namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;

/// <summary>
///     Checkout session created by an external provider.
/// </summary>
public record CheckoutSessionProviderResult(
    string Provider,
    string SessionId,
    string CheckoutUrl);
