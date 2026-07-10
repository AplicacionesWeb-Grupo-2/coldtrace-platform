using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     AI-generated resolution plan persisted for operator review and audit.
/// </summary>
public class AiResolutionPlan : IAuditableEntity
{
    public const string StatusPending = "pending";
    public const string StatusApproved = "approved";
    public const string StatusRejected = "rejected";

    protected AiResolutionPlan()
    {
        Status = string.Empty;
        Summary = string.Empty;
        ProbableCause = string.Empty;
        CorrectiveActionDraft = string.Empty;
        ResolutionNotesDraft = string.Empty;
        EscalationUrgency = string.Empty;
        EscalationReason = string.Empty;
        ModelProvider = string.Empty;
        ModelName = string.Empty;
    }

    public AiResolutionPlan(CreateAiResolutionPlanCommand command)
    {
        OrganizationId = command.OrganizationId;
        IncidentId = command.IncidentId;
        Status = StatusPending;
        Summary = command.Summary.Trim();
        ProbableCause = command.ProbableCause.Trim();
        RecommendedSteps = command.RecommendedSteps
            .OrderBy(step => step.Sequence)
            .Select(step => new AiResolutionPlanStep(
                step.Sequence,
                step.Action,
                step.Rationale,
                step.ExpectedOutcome))
            .ToList();
        CorrectiveActionDraft = command.CorrectiveActionDraft.Trim();
        ResolutionNotesDraft = command.ResolutionNotesDraft.Trim();
        EscalationRecommended = command.EscalationRecommended;
        EscalationUrgency = command.EscalationUrgency.Trim();
        EscalationReason = command.EscalationReason.Trim();
        RequiredEvidenceItems = command.RequiredEvidence
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => new AiResolutionPlanRequiredEvidence(value))
            .ToList();
        UncertaintyNoteItems = command.UncertaintyNotes
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => new AiResolutionPlanUncertaintyNote(value))
            .ToList();
        ModelProvider = command.ModelProvider.Trim();
        ModelName = command.ModelName.Trim();
        ProviderMetadata = string.IsNullOrWhiteSpace(command.ProviderMetadata)
            ? null
            : command.ProviderMetadata.Trim();
        GeneratedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public int OrganizationId { get; private set; }

    public int IncidentId { get; private set; }

    public string Status { get; private set; }

    public string Summary { get; private set; }

    public string ProbableCause { get; private set; }

    public List<AiResolutionPlanStep> RecommendedSteps { get; private set; } = [];

    public string CorrectiveActionDraft { get; private set; }

    public string ResolutionNotesDraft { get; private set; }

    public bool EscalationRecommended { get; private set; }

    public string EscalationUrgency { get; private set; }

    public string EscalationReason { get; private set; }

    public List<AiResolutionPlanRequiredEvidence> RequiredEvidenceItems { get; private set; } = [];

    public List<AiResolutionPlanUncertaintyNote> UncertaintyNoteItems { get; private set; } = [];

    public string ModelProvider { get; private set; }

    public string ModelName { get; private set; }

    public string? ProviderMetadata { get; private set; }

    public DateTimeOffset GeneratedAt { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public string? ApprovedBy { get; private set; }

    public DateTimeOffset? RejectedAt { get; private set; }

    public string? RejectedBy { get; private set; }

    public string? RejectionReason { get; private set; }

    public string? FinalCorrectiveAction { get; private set; }

    public string? FinalResolutionNotes { get; private set; }

    public Incident Incident { get; private set; } = null!;

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public bool IsPending() => Status == StatusPending;

    public void Approve(ApproveAiResolutionPlanCommand command)
    {
        Status = StatusApproved;
        ApprovedAt = DateTimeOffset.UtcNow;
        ApprovedBy = command.ApprovedBy;
        FinalCorrectiveAction = command.FinalCorrectiveAction;
        FinalResolutionNotes = command.FinalResolutionNotes;
    }

    public void Reject(RejectAiResolutionPlanCommand command)
    {
        Status = StatusRejected;
        RejectedAt = DateTimeOffset.UtcNow;
        RejectedBy = command.RejectedBy;
        RejectionReason = command.RejectionReason;
    }
}
