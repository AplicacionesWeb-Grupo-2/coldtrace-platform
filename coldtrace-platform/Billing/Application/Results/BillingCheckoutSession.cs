namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     Provider-hosted checkout session returned to the frontend.
/// </summary>
public record BillingCheckoutSession(
    string Provider,
    string SessionId,
    string CheckoutUrl,
    string TargetPlanCode);
