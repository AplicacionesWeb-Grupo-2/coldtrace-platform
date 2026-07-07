using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing a subscription plan.
/// </summary>
[SwaggerSchema(Description = "A subscription plan resource")]
public record SubscriptionPlanResource(
    [SwaggerParameter(Description = "Plan identifier")]
    int Id,
    [SwaggerParameter(Description = "Stable plan code")]
    string Code,
    [SwaggerParameter(Description = "User-facing plan name")]
    string DisplayName,
    [SwaggerParameter(Description = "User-facing plan description")]
    string Description,
    [SwaggerParameter(Description = "Monthly price in minor units")]
    int MonthlyPriceCents,
    [SwaggerParameter(Description = "ISO currency code")]
    string Currency,
    [SwaggerParameter(Description = "Optional Stripe price identifier")]
    string? StripePriceId,
    [SwaggerParameter(Description = "Whether this plan should be highlighted")]
    bool Recommended,
    [SwaggerParameter(Description = "Optional highlight label")]
    string? RecommendedLabel,
    [SwaggerParameter(Description = "Whether this plan is visible/selectable by clients")]
    bool Visible,
    [SwaggerParameter(Description = "Usage limit matrix")]
    SubscriptionPlanUsageLimitsResource UsageLimits,
    [SwaggerParameter(Description = "Feature entitlement flags")]
    SubscriptionPlanFeatureFlagsResource FeatureFlags,
    [SwaggerParameter(Description = "User-facing feature list")]
    IReadOnlyCollection<string> IncludedFeatures);
