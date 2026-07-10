using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

namespace ColdTrace.Platform.AiAssistance.Application.Results;

/// <summary>
///     Advisory dashboard interpretation generated from backend-owned evidence.
/// </summary>
public record DashboardAiInterpretation(
    int OrganizationId,
    string? Question,
    DashboardInterpretationOutput Interpretation,
    IReadOnlyCollection<DashboardSourceMetricOutput> SourceMetrics,
    string ModelProvider,
    string ModelName,
    DateTimeOffset GeneratedAt);
