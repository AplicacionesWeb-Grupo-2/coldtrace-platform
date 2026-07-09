using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AiAssistance.Interfaces.REST.Transform;

/// <summary>
///     Assembles dashboard AI interpretation resources from application results.
/// </summary>
public static class DashboardAiInterpretationResourceFromResultAssembler
{
    public static DashboardAiInterpretationResource ToResourceFromResult(DashboardAiInterpretation result) =>
        new(
            result.OrganizationId,
            result.Question,
            result.GeneratedAt,
            result.Interpretation.OverallReading,
            result.Interpretation.AttentionLevel,
            result.Interpretation.MetricInsights.Select(ToInsightResource).ToList(),
            result.Interpretation.Risks.ToList(),
            result.Interpretation.RecommendedActions.ToList(),
            result.Interpretation.UncertaintyNotes.ToList(),
            result.SourceMetrics.Select(ToSourceMetricResource).ToList(),
            result.ModelProvider,
            result.ModelName);

    private static DashboardAiInterpretationInsightResource ToInsightResource(
        DashboardInterpretationInsightOutput insight) =>
        new(insight.Title, insight.Metric, insight.Interpretation, insight.Severity);

    private static DashboardAiSourceMetricResource ToSourceMetricResource(DashboardSourceMetricOutput metric) =>
        new(metric.Name, metric.Value, metric.Unit, metric.Description);
}
