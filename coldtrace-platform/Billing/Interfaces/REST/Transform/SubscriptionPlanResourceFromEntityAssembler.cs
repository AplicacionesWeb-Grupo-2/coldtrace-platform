using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles subscription plan resources from aggregates.
/// </summary>
public static class SubscriptionPlanResourceFromEntityAssembler
{
    public static SubscriptionPlanResource ToResourceFromEntity(SubscriptionPlan plan) =>
        new(
            plan.Id,
            plan.Code,
            plan.DisplayName,
            plan.Description,
            plan.MonthlyPriceCents,
            plan.Currency,
            plan.StripePriceId,
            plan.Recommended,
            plan.RecommendedLabel,
            plan.Active,
            new SubscriptionPlanUsageLimitsResource(
                plan.UsageLimits.MaxLocations,
                plan.UsageLimits.MaxAssets,
                plan.UsageLimits.MaxIotDevices,
                plan.UsageLimits.MaxUsers,
                plan.UsageLimits.HistoryRetentionDays),
            new SubscriptionPlanFeatureFlagsResource(
                plan.FeatureFlags.AllowsExports,
                plan.FeatureFlags.AllowsMaintenance,
                plan.FeatureFlags.AllowsAiGuidance,
                plan.FeatureFlags.AllowsAiReportSummary),
            plan.IncludedFeatures.ToList());
}
