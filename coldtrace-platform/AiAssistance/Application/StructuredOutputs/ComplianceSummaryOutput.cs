namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract for compliance report summaries.
/// </summary>
public record ComplianceSummaryOutput(
    string ComplianceStatus,
    string Summary,
    IReadOnlyCollection<string> KeyRisks,
    IReadOnlyCollection<string> CriticalAssets,
    IReadOnlyCollection<string> CorrectiveActions,
    IReadOnlyCollection<string> NextSteps);
