namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract for incident resolution plans.
/// </summary>
public record IncidentResolutionPlanOutput(
    string Summary,
    string ProbableCause,
    string RiskLevel,
    IReadOnlyCollection<string> RecommendedSteps,
    IReadOnlyCollection<string> RequiredEvidence,
    string CorrectiveActionDraft);
