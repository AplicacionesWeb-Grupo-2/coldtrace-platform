namespace ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;

/// <summary>
///     Structured AI output contract aligned with the incident AI resolution plan API resource.
/// </summary>
public record IncidentResolutionPlanOutput(
    string Summary,
    string ProbableCause,
    IReadOnlyCollection<IncidentResolutionPlanStepOutput> RecommendedSteps,
    string CorrectiveActionDraft,
    string ResolutionNotesDraft,
    bool EscalationRecommended,
    string EscalationUrgency,
    string EscalationReason,
    IReadOnlyCollection<string> RequiredEvidence,
    IReadOnlyCollection<string> UncertaintyNotes);

/// <summary>
///     Structured AI output contract for one incident resolution plan step.
/// </summary>
public record IncidentResolutionPlanStepOutput(
    int Sequence,
    string Action,
    string Rationale,
    string ExpectedOutcome);
