using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing an organization's active subscription.
/// </summary>
[SwaggerSchema(Description = "An organization subscription resource")]
public record OrganizationSubscriptionResource(
    [SwaggerParameter(Description = "Subscription identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Subscription status")]
    string Status,
    [SwaggerParameter(Description = "Billing provider")]
    string Provider,
    [SwaggerParameter(Description = "Provider customer identifier")]
    string? ProviderCustomerId,
    [SwaggerParameter(Description = "Provider subscription identifier")]
    string? ProviderSubscriptionId,
    [SwaggerParameter(Description = "Current billing period start")]
    DateTimeOffset? CurrentPeriodStart,
    [SwaggerParameter(Description = "Current billing period end")]
    DateTimeOffset? CurrentPeriodEnd,
    [SwaggerParameter(Description = "Whether cancellation is scheduled")]
    bool CancelAtPeriodEnd,
    [SwaggerParameter(Description = "Provider synchronization metadata")]
    string? Metadata,
    [SwaggerParameter(Description = "Subscribed plan")]
    SubscriptionPlanResource Plan,
    [SwaggerParameter(Description = "Current usage counters")]
    OrganizationSubscriptionUsageResource Usage,
    [SwaggerParameter(Description = "Computed entitlements")]
    IReadOnlyCollection<OrganizationEntitlementResource> Entitlements);
