using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles organization subscription resources from application results.
/// </summary>
public static class OrganizationSubscriptionResourceFromResultAssembler
{
    public static OrganizationSubscriptionResource ToResourceFromResult(OrganizationSubscriptionDetails details) =>
        new(
            details.Subscription.Id,
            details.Subscription.OrganizationId,
            details.Subscription.Status,
            details.Subscription.Provider,
            details.Subscription.ProviderCustomerId,
            details.Subscription.ProviderSubscriptionId,
            details.Subscription.CurrentPeriodStart,
            details.Subscription.CurrentPeriodEnd,
            details.Subscription.CancelAtPeriodEnd,
            details.Subscription.Metadata,
            SubscriptionPlanResourceFromEntityAssembler.ToResourceFromEntity(details.Plan),
            new OrganizationSubscriptionUsageResource(
                details.Usage.Locations,
                details.Usage.Assets,
                details.Usage.IotDevices,
                details.Usage.Users),
            details.Entitlements.Select(ToResourceFromEntitlement).ToList());

    private static OrganizationEntitlementResource ToResourceFromEntitlement(
        OrganizationEntitlement entitlement) =>
        new(
            entitlement.Key,
            entitlement.Category,
            entitlement.Enabled,
            entitlement.Limit,
            entitlement.Used,
            entitlement.Remaining,
            entitlement.LockedReason);
}
