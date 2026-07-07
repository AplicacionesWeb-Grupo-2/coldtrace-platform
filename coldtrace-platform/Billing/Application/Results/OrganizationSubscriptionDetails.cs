using ColdTrace.Platform.Billing.Domain.Model.Aggregates;

namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     Subscription snapshot returned by the billing application layer.
/// </summary>
public record OrganizationSubscriptionDetails(
    OrganizationSubscription Subscription,
    SubscriptionPlan Plan,
    OrganizationSubscriptionUsage Usage,
    IReadOnlyCollection<OrganizationEntitlement> Entitlements);
