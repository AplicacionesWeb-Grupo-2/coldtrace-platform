using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest.Resources;

/// <summary>
///     Structured advisory interpretation of an organization dashboard.
/// </summary>
public record DashboardAiInterpretationResource(
    [SwaggerParameter(Description = "Organization that owns the source dashboard data")]
    int OrganizationId,
    [SwaggerParameter(Description = "Optional operator question used for generation")]
    string? Question,
    [SwaggerParameter(Description = "UTC generation timestamp")]
    DateTimeOffset GeneratedAt,
    [SwaggerParameter(Description = "Concise operational reading")]
    string OverallReading,
    [SwaggerParameter(Description = "Operator-facing attention level")]
    string AttentionLevel,
    [SwaggerParameter(Description = "Structured metric-level insights")]
    IReadOnlyCollection<DashboardAiInterpretationInsightResource> MetricInsights,
    [SwaggerParameter(Description = "Operational risks inferred from source metrics")]
    IReadOnlyCollection<string> Risks,
    [SwaggerParameter(Description = "Advisory next actions")]
    IReadOnlyCollection<string> RecommendedActions,
    [SwaggerParameter(Description = "Assumptions or missing-context notes")]
    IReadOnlyCollection<string> UncertaintyNotes,
    [SwaggerParameter(Description = "Backend-owned factual dashboard metrics used as source references")]
    IReadOnlyCollection<DashboardAiSourceMetricResource> SourceMetrics,
    [SwaggerParameter(Description = "Configured AI provider")]
    string ModelProvider,
    [SwaggerParameter(Description = "Configured AI model")]
    string ModelName);

/// <summary>
///     One structured dashboard metric insight.
/// </summary>
public record DashboardAiInterpretationInsightResource(
    string Title,
    string Metric,
    string Interpretation,
    string Severity);

/// <summary>
///     One factual dashboard metric assembled by the backend.
/// </summary>
public record DashboardAiSourceMetricResource(
    string Name,
    string Value,
    string? Unit,
    string Description);
