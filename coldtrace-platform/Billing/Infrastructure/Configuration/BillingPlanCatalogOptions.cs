namespace ColdTrace.Platform.Billing.Infrastructure.Configuration;

/// <summary>
///     Configurable provider identifiers for public billing plans.
/// </summary>
public sealed class BillingPlanCatalogOptions
{
    public string? OperationsStripePriceId { get; set; }

    public string? ComplianceAiStripePriceId { get; set; }
}
