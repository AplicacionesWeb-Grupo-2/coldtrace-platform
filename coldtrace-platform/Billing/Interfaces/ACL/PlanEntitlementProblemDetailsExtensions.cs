using Microsoft.AspNetCore.Mvc;

namespace ColdTrace.Platform.Billing.Interfaces.ACL;

/// <summary>
///     Maps entitlement decisions to neutral RFC 7807 extension properties.
/// </summary>
public static class PlanEntitlementProblemDetailsExtensions
{
    public static void AppendPlanEntitlementProperties(
        this ProblemDetails problemDetails,
        EntitlementCheckSnapshot entitlement)
    {
        PutIfPresent(problemDetails.Extensions, "organizationId", entitlement.OrganizationId);
        PutIfPresent(problemDetails.Extensions, "planCode", entitlement.PlanCode);
        PutIfPresent(problemDetails.Extensions, "subscriptionStatus", entitlement.SubscriptionStatus);
        PutIfPresent(problemDetails.Extensions, "entitlementKey", entitlement.Key);
        PutIfPresent(problemDetails.Extensions, "entitlementCategory", entitlement.Category);
        PutIfPresent(problemDetails.Extensions, "entitlementEnabled", entitlement.Enabled);
        PutIfPresent(problemDetails.Extensions, "limit", entitlement.Limit);
        PutIfPresent(problemDetails.Extensions, "used", entitlement.Used);
        PutIfPresent(problemDetails.Extensions, "remaining", entitlement.Remaining);
        PutIfPresent(problemDetails.Extensions, "lockedReason", entitlement.LockedReason);
        PutIfPresent(problemDetails.Extensions, "requiredPlanCode", entitlement.RequiredPlanCode);
    }

    private static void PutIfPresent(
        IDictionary<string, object?> extensions,
        string key,
        object? value)
    {
        if (value is not null) extensions[key] = value;
    }
}
