using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Application.ACL;

/// <summary>
///     Application-layer implementation of the billing anti-corruption facade.
/// </summary>
public class SubscriptionBillingContextFacade(
    IOrganizationSubscriptionCommandService organizationSubscriptionCommandService,
    IOrganizationSubscriptionQueryService organizationSubscriptionQueryService,
    ISubscriptionPlanQueryService subscriptionPlanQueryService)
    : ISubscriptionBillingContextFacade
{
    public async Task InitializeBaseSubscriptionForOrganizationAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        await organizationSubscriptionCommandService.Handle(
            new InitializeBaseOrganizationSubscriptionCommand(organizationId),
            cancellationToken);
    }

    public async Task<OrganizationEntitlementsSnapshot?> FetchEntitlementsByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        if (organizationId <= 0) return null;

        var result = await organizationSubscriptionQueryService.Handle(
            new GetOrganizationSubscriptionByOrganizationIdQuery(organizationId),
            cancellationToken);

        return result switch
        {
            Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Success success =>
                new OrganizationEntitlementsSnapshot(
                    success.Value.Subscription.OrganizationId,
                    success.Value.Subscription.PlanCode,
                    success.Value.Subscription.Status,
                    success.Value.Entitlements.Select(ToSnapshot).ToList()),
            _ => null
        };
    }

    public async Task<EntitlementCheckSnapshot?> CheckEntitlementAsync(
        int organizationId,
        string entitlementKey,
        CancellationToken cancellationToken = default)
    {
        if (organizationId <= 0 || string.IsNullOrWhiteSpace(entitlementKey)) return null;

        var normalizedKey = entitlementKey.Trim().ToLowerInvariant();
        var snapshot = await FetchEntitlementsByOrganizationIdAsync(organizationId, cancellationToken);
        if (snapshot is null)
            return await UnavailableEntitlementAsync(
                organizationId,
                null,
                null,
                normalizedKey,
                "Organization subscription is not configured",
                cancellationToken);

        var entitlement = snapshot.Entitlements.FirstOrDefault(item => item.Key == normalizedKey);
        return entitlement is null
            ? await UnavailableEntitlementAsync(
                organizationId,
                snapshot.PlanCode,
                snapshot.Status,
                normalizedKey,
                "Entitlement is not configured for this operation",
                cancellationToken)
            : await ToCheckSnapshotAsync(snapshot, entitlement, cancellationToken);
    }

    public async Task<bool> CanUseEntitlementAsync(
        int organizationId,
        string entitlementKey,
        CancellationToken cancellationToken = default) =>
        (await CheckEntitlementAsync(organizationId, entitlementKey, cancellationToken))?.Enabled ?? false;

    private static EntitlementItemSnapshot ToSnapshot(OrganizationEntitlement entitlement) =>
        new(
            entitlement.Key,
            entitlement.Category,
            entitlement.Enabled,
            entitlement.Limit,
            entitlement.Used,
            entitlement.Remaining,
            entitlement.LockedReason);

    private async Task<EntitlementCheckSnapshot> ToCheckSnapshotAsync(
        OrganizationEntitlementsSnapshot snapshot,
        EntitlementItemSnapshot entitlement,
        CancellationToken cancellationToken) =>
        new(
            snapshot.OrganizationId,
            snapshot.PlanCode,
            snapshot.Status,
            entitlement.Key,
            entitlement.Category,
            entitlement.Enabled,
            entitlement.Limit,
            entitlement.Used,
            entitlement.Remaining,
            entitlement.LockedReason,
            entitlement.Enabled
                ? null
                : await RequiredPlanCodeForAsync(entitlement.Key, entitlement.Used, cancellationToken));

    private async Task<EntitlementCheckSnapshot> UnavailableEntitlementAsync(
        int organizationId,
        string? planCode,
        string? subscriptionStatus,
        string key,
        string lockedReason,
        CancellationToken cancellationToken) =>
        new(
            organizationId,
            planCode,
            subscriptionStatus,
            key,
            null,
            false,
            null,
            null,
            null,
            lockedReason,
            await RequiredPlanCodeForAsync(key, null, cancellationToken));

    private async Task<string?> RequiredPlanCodeForAsync(
        string entitlementKey,
        int? used,
        CancellationToken cancellationToken)
    {
        var plans = await subscriptionPlanQueryService.Handle(
            new GetActiveSubscriptionPlansQuery(),
            cancellationToken);
        return plans
            .Where(plan => PlanAllowsEntitlement(plan, entitlementKey, used))
            .Select(plan => plan.Code)
            .FirstOrDefault();
    }

    private static bool PlanAllowsEntitlement(
        Domain.Model.Aggregates.SubscriptionPlan plan,
        string entitlementKey,
        int? used) =>
        entitlementKey switch
        {
            EntitlementKeys.Locations => CapacityAllows(plan.UsageLimits.MaxLocations, used),
            EntitlementKeys.Assets => CapacityAllows(plan.UsageLimits.MaxAssets, used),
            EntitlementKeys.IotDevices => CapacityAllows(plan.UsageLimits.MaxIotDevices, used),
            EntitlementKeys.Users => CapacityAllows(plan.UsageLimits.MaxUsers, used),
            EntitlementKeys.ReportHistory => plan.UsageLimits.HistoryRetentionDays is not null,
            EntitlementKeys.Maintenance => plan.FeatureFlags.AllowsMaintenance,
            EntitlementKeys.AiGuidance => plan.FeatureFlags.AllowsAiGuidance,
            EntitlementKeys.AiReportSummary => plan.FeatureFlags.AllowsAiReportSummary,
            _ => false
        };

    private static bool CapacityAllows(int? limit, int? used) =>
        limit is null || used is null || used < limit;
}
