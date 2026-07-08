namespace ColdTrace.Platform.Billing.Interfaces.ACL;

/// <summary>
///     Shared entitlement guard used before restricted writes or paid AI operations.
/// </summary>
public static class PlanEntitlementGuard
{
    public static async Task EnsureEntitlementAsync(
        this ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
        int organizationId,
        string entitlementKey,
        string messageResourceKey,
        CancellationToken cancellationToken = default)
    {
        var entitlement = await subscriptionBillingContextFacade.CheckEntitlementAsync(
            organizationId,
            entitlementKey,
            cancellationToken);

        if (entitlement is { Enabled: false })
            throw new PlanLimitExceededException(messageResourceKey, entitlement);
    }
}
