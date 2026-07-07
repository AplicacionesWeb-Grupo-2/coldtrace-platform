namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command containing a generated resolution plan ready to persist.
/// </summary>
public record CreateAiResolutionPlanCommand(
    int OrganizationId,
    int IncidentId,
    string Summary,
    string ProbableCause,
    IReadOnlyCollection<CreateAiResolutionPlanStepCommand> RecommendedSteps,
    string CorrectiveActionDraft,
    string ResolutionNotesDraft,
    bool EscalationRecommended,
    string EscalationUrgency,
    string EscalationReason,
    IReadOnlyCollection<string> RequiredEvidence,
    IReadOnlyCollection<string> UncertaintyNotes,
    string ModelProvider,
    string ModelName,
    string? ProviderMetadata);

/// <summary>
///     Command containing one generated resolution step.
/// </summary>
public record CreateAiResolutionPlanStepCommand(
    int Sequence,
    string Action,
    string Rationale,
    string ExpectedOutcome);
