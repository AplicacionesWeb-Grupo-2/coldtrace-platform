using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an AI resolution plan resource from an aggregate.
/// </summary>
public static class AiResolutionPlanResourceFromEntityAssembler
{
    public static AiResolutionPlanResource ToResourceFromEntity(AiResolutionPlan plan) =>
        new(
            plan.Id,
            plan.OrganizationId,
            plan.IncidentId,
            plan.Status,
            plan.Summary,
            plan.ProbableCause,
            plan.RecommendedSteps
                .OrderBy(step => step.Sequence)
                .Select(step => new AiResolutionPlanStepResource(
                    step.Sequence,
                    step.Action,
                    step.Rationale,
                    step.ExpectedOutcome))
                .ToList(),
            plan.CorrectiveActionDraft,
            plan.ResolutionNotesDraft,
            plan.EscalationRecommended,
            plan.EscalationUrgency,
            plan.EscalationReason,
            plan.RequiredEvidenceItems.Select(item => item.Value).ToList(),
            plan.UncertaintyNoteItems.Select(item => item.Value).ToList(),
            plan.ModelProvider,
            plan.ModelName,
            plan.ProviderMetadata,
            plan.GeneratedAt,
            plan.ApprovedAt,
            plan.ApprovedBy,
            plan.RejectedAt,
            plan.RejectedBy,
            plan.RejectionReason,
            plan.FinalCorrectiveAction,
            plan.FinalResolutionNotes);
}
