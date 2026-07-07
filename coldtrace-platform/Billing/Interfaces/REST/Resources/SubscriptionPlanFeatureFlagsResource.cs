using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing subscription plan feature flags.
/// </summary>
[SwaggerSchema(Description = "Subscription plan feature flags")]
public record SubscriptionPlanFeatureFlagsResource(
    [SwaggerParameter(Description = "Whether export workflows are included")]
    bool AllowsExports,
    [SwaggerParameter(Description = "Whether maintenance workflows are included")]
    bool AllowsMaintenance,
    [SwaggerParameter(Description = "Whether AI incident guidance is included")]
    bool AllowsAiGuidance,
    [SwaggerParameter(Description = "Whether AI report summaries are included")]
    bool AllowsAiReportSummary);
