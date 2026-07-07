using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Billing.Application.Internal.Services;

/// <summary>
///     Computes effective entitlements from subscription status, plan limits and usage.
/// </summary>
public class EntitlementPolicyService
{
    public IReadOnlyCollection<OrganizationEntitlement> Compute(
        OrganizationSubscription subscription,
        SubscriptionPlan plan,
        OrganizationSubscriptionUsage usage) =>
    [
        Limit(EntitlementKeys.Locations, plan.DisplayName, subscription, plan.UsageLimits.MaxLocations,
            usage.Locations),
        Limit(EntitlementKeys.Assets, plan.DisplayName, subscription, plan.UsageLimits.MaxAssets,
            usage.Assets),
        Limit(EntitlementKeys.IotDevices, plan.DisplayName, subscription, plan.UsageLimits.MaxIotDevices,
            usage.IotDevices),
        Limit(EntitlementKeys.Users, plan.DisplayName, subscription, plan.UsageLimits.MaxUsers,
            usage.Users),
        Limit(EntitlementKeys.ReportHistory, plan.DisplayName, subscription,
            plan.UsageLimits.HistoryRetentionDays, null),
        Feature(EntitlementKeys.Exports, plan.DisplayName, subscription, plan.FeatureFlags.AllowsExports),
        Feature(EntitlementKeys.Maintenance, plan.DisplayName, subscription, plan.FeatureFlags.AllowsMaintenance),
        Feature(EntitlementKeys.AiGuidance, plan.DisplayName, subscription, plan.FeatureFlags.AllowsAiGuidance),
        Feature(EntitlementKeys.AiReportSummary, plan.DisplayName, subscription,
            plan.FeatureFlags.AllowsAiReportSummary)
    ];

    private static OrganizationEntitlement Limit(
        string key,
        string planName,
        OrganizationSubscription subscription,
        int? limit,
        int? used)
    {
        var statusAllows = subscription.AllowsPlanEntitlements();
        var remaining = Remaining(limit, used);
        var limitAvailable = limit is null || used is null || used < limit;
        var enabled = statusAllows && limitAvailable;

        return new OrganizationEntitlement(
            key,
            EntitlementCategories.Limit,
            enabled,
            limit,
            used,
            remaining,
            LockedReason(enabled, statusAllows, limitAvailable, planName, subscription));
    }

    private static OrganizationEntitlement Feature(
        string key,
        string planName,
        OrganizationSubscription subscription,
        bool included)
    {
        var statusAllows = subscription.AllowsPlanEntitlements();
        var enabled = statusAllows && included;

        return new OrganizationEntitlement(
            key,
            EntitlementCategories.Feature,
            enabled,
            null,
            null,
            null,
            LockedReason(enabled, statusAllows, included, planName, subscription));
    }

    private static int? Remaining(int? limit, int? used) =>
        limit is null || used is null ? null : Math.Max(limit.Value - used.Value, 0);

    private static string? LockedReason(
        bool enabled,
        bool statusAllows,
        bool planAllows,
        string planName,
        OrganizationSubscription subscription)
    {
        if (enabled) return null;
        if (!statusAllows)
            return $"Subscription status {subscription.Status} does not unlock plan entitlements";
        if (!planAllows)
            return $"Current {planName} plan does not include available capacity for this entitlement";

        return "Entitlement is unavailable for the current subscription";
    }
}
