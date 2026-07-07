namespace ColdTrace.Platform.Billing.Infrastructure.Configuration;

/// <summary>
///     Environment-driven billing configuration.
/// </summary>
public sealed class BillingOptions
{
    public const string SectionName = "Billing";

    public BillingPlanCatalogOptions PlanCatalog { get; set; } = new();

    public BillingStripeOptions Stripe { get; set; } = new();

    public void ExpandEnvironmentVariables()
    {
        PlanCatalog.OperationsStripePriceId = NormalizeOptionalValue(
            ExpandValue(PlanCatalog.OperationsStripePriceId));
        PlanCatalog.ComplianceAiStripePriceId = NormalizeOptionalValue(
            ExpandValue(PlanCatalog.ComplianceAiStripePriceId));
        Stripe.SecretKey = NormalizeOptionalValue(ExpandValue(Stripe.SecretKey));
        Stripe.WebhookSigningSecret = NormalizeOptionalValue(ExpandValue(Stripe.WebhookSigningSecret));
        Stripe.CheckoutSuccessUrl = NormalizeOptionalValue(ExpandValue(Stripe.CheckoutSuccessUrl));
        Stripe.CheckoutCancelUrl = NormalizeOptionalValue(ExpandValue(Stripe.CheckoutCancelUrl));
        Stripe.CustomerPortalReturnUrl = NormalizeOptionalValue(ExpandValue(Stripe.CustomerPortalReturnUrl));
    }

    private static string? ExpandValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? value : Environment.ExpandEnvironmentVariables(value);

    private static string? NormalizeOptionalValue(string? value) =>
        HasConfiguredValue(value) ? value!.Trim() : null;

    private static bool HasConfiguredValue(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        !value.Contains('%', StringComparison.Ordinal) &&
        !value.StartsWith("$", StringComparison.Ordinal);
}
