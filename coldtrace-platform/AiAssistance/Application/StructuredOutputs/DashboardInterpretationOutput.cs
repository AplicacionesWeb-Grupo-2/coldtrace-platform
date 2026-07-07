namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract aligned with the dashboard AI interpretation API resource.
/// </summary>
public record DashboardInterpretationOutput(
    string OverallReading,
    string AttentionLevel,
    IReadOnlyCollection<DashboardInterpretationInsightOutput> MetricInsights,
    IReadOnlyCollection<string> Risks,
    IReadOnlyCollection<string> RecommendedActions,
    IReadOnlyCollection<string> UncertaintyNotes,
    IReadOnlyCollection<DashboardSourceMetricOutput> SourceMetrics);

/// <summary>
///     Structured AI output contract for one dashboard metric insight.
/// </summary>
public record DashboardInterpretationInsightOutput(
    string Title,
    string Metric,
    string Interpretation,
    string Severity);

/// <summary>
///     Factual dashboard metric used as prompt evidence and response provenance.
/// </summary>
public record DashboardSourceMetricOutput(
    string Name,
    string Value,
    string? Unit,
    string Description);
