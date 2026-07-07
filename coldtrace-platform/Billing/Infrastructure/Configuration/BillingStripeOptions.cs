namespace ColdTrace.Platform.Billing.Infrastructure.Configuration;

/// <summary>
///     Environment-driven Stripe checkout configuration.
/// </summary>
public sealed class BillingStripeOptions
{
    public string? SecretKey { get; set; }

    public string? WebhookSigningSecret { get; set; }

    public string? CheckoutSuccessUrl { get; set; }

    public string? CheckoutCancelUrl { get; set; }

    public string? CustomerPortalReturnUrl { get; set; }

    public bool HasCheckoutConfiguration() =>
        SecretKey is not null && CheckoutSuccessUrl is not null && CheckoutCancelUrl is not null;

    public bool HasWebhookConfiguration() => WebhookSigningSecret is not null;

    public bool HasCustomerPortalConfiguration() =>
        SecretKey is not null && CustomerPortalReturnUrl is not null;
}
