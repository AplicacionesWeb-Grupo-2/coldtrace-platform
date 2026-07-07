using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.Billing.Application.Internal.QueryServices;

/// <summary>
///     Application service implementation for subscription plan queries.
/// </summary>
public class SubscriptionPlanQueryService(
    IOptions<BillingOptions> options,
    ILogger<SubscriptionPlanQueryService> logger)
    : ISubscriptionPlanQueryService
{
    public Task<IReadOnlyCollection<SubscriptionPlan>> Handle(
        GetActiveSubscriptionPlansQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Querying active subscription plans");
        IReadOnlyCollection<SubscriptionPlan> plans = DefaultPlans(options.Value.PlanCatalog)
            .Where(plan => plan.Active)
            .ToList();
        return Task.FromResult(plans);
    }

    private static IReadOnlyCollection<SubscriptionPlan> DefaultPlans(BillingPlanCatalogOptions planCatalog) =>
    [
        new SubscriptionPlan(
            1,
            "base",
            "Base",
            "For small teams validating cold-chain monitoring.",
            "PEN",
            0,
            null,
            false,
            null,
            true,
            new PlanUsageLimits(1, 2, 3, 3, 7),
            new PlanFeatureFlags(false, false, false, false),
            ["Basic monitoring", "In-app alerts", "Incident list", "Basic daily log"]),
        new SubscriptionPlan(
            2,
            "operations",
            "Operations",
            "For SMEs with recurring cold-chain monitoring and operational reporting.",
            "PEN",
            14900,
            planCatalog.OperationsStripePriceId,
            true,
            "Recommended",
            true,
            new PlanUsageLimits(3, 20, 50, 10, 365),
            new PlanFeatureFlags(true, true, false, false),
            ["Email alerts", "Operational reports", "Maintenance scheduling", "CSV exports", "Full incident lifecycle"]),
        new SubscriptionPlan(
            3,
            "compliance-ai",
            "Compliance AI",
            "For multi-site quality teams that need compliance evidence and AI guidance.",
            "PEN",
            39900,
            planCatalog.ComplianceAiStripePriceId,
            false,
            null,
            true,
            new PlanUsageLimits(10, 100, 250, 30, 730),
            new PlanFeatureFlags(true, true, true, true),
            ["Advanced compliance reports", "AI incident guidance", "AI report summaries", "Priority support", "Expanded exports"])
    ];
}
