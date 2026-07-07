using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Billing.Domain.Model.Aggregates;

/// <summary>
///     Subscription plan aggregate for the billing bounded context.
/// </summary>
public class SubscriptionPlan
{
    public SubscriptionPlan(
        int id,
        string code,
        string displayName,
        string description,
        string currency,
        int monthlyPriceCents,
        string? stripePriceId,
        bool recommended,
        string? recommendedLabel,
        bool active,
        PlanUsageLimits usageLimits,
        PlanFeatureFlags featureFlags,
        IEnumerable<string> includedFeatures)
    {
        if (id <= 0)
            throw new ArgumentException("Plan identifier must be positive.", nameof(id));
        if (monthlyPriceCents < 0)
            throw new ArgumentException("Monthly price must be zero or greater.", nameof(monthlyPriceCents));

        Id = id;
        Code = RequireText(code, nameof(code)).ToLowerInvariant();
        DisplayName = RequireText(displayName, nameof(displayName));
        Description = RequireText(description, nameof(description));
        Currency = RequireText(currency, nameof(currency)).ToUpperInvariant();
        MonthlyPriceCents = monthlyPriceCents;
        StripePriceId = NormalizeOptionalText(stripePriceId);
        Recommended = recommended;
        RecommendedLabel = NormalizeOptionalText(recommendedLabel);
        Active = active;
        UsageLimits = usageLimits ?? throw new ArgumentNullException(nameof(usageLimits));
        FeatureFlags = featureFlags ?? throw new ArgumentNullException(nameof(featureFlags));
        IncludedFeatures = includedFeatures
            .Where(feature => !string.IsNullOrWhiteSpace(feature))
            .Select(feature => feature.Trim())
            .ToList();
    }

    public int Id { get; }

    public string Code { get; }

    public string DisplayName { get; }

    public string Description { get; }

    public string Currency { get; }

    public int MonthlyPriceCents { get; }

    public string? StripePriceId { get; }

    public bool Recommended { get; }

    public string? RecommendedLabel { get; }

    public bool Active { get; }

    public PlanUsageLimits UsageLimits { get; }

    public PlanFeatureFlags FeatureFlags { get; }

    public IReadOnlyCollection<string> IncludedFeatures { get; }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.", fieldName);

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
