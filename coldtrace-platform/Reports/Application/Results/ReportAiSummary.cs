using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;

namespace ColdTrace.Platform.Reports.Application.Results;

/// <summary>
///     Advisory AI summary generated from a persisted report and related evidence.
/// </summary>
public record ReportAiSummary(
    Report Report,
    ComplianceSummaryOutput Summary,
    string ModelProvider,
    string ModelName,
    DateTimeOffset SummaryGeneratedAt);
