namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract for operational dashboard interpretation.
/// </summary>
public record DashboardInterpretationOutput(
    string Summary,
    IReadOnlyCollection<string> Risks,
    IReadOnlyCollection<string> CriticalAssets,
    IReadOnlyCollection<string> ComplianceSignals,
    IReadOnlyCollection<string> RecommendedNextSteps);
