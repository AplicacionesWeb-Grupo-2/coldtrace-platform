namespace ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Feature flags exposed by a subscription plan.
/// </summary>
public record PlanFeatureFlags(
    bool AllowsExports,
    bool AllowsMaintenance,
    bool AllowsAiGuidance,
    bool AllowsAiReportSummary);
