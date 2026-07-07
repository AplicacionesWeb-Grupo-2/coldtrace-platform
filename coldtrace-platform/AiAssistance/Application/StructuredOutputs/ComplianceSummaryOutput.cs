namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract aligned with the report AI summary API resource.
/// </summary>
public record ComplianceSummaryOutput(
    string ExecutiveSummary,
    IReadOnlyCollection<ComplianceFindingOutput> Findings,
    IReadOnlyCollection<string> EvidenceGaps,
    IReadOnlyCollection<string> RecommendedActions,
    IReadOnlyCollection<string> UncertaintyNotes);

/// <summary>
///     Structured AI output contract for one compliance or operational finding.
/// </summary>
public record ComplianceFindingOutput(
    string Area,
    string Status,
    string Evidence,
    string Recommendation);
