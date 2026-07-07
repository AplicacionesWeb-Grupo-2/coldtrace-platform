using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing an AI-generated incident resolution plan.
/// </summary>
[SwaggerSchema(Description = "An AI-generated resolution plan resource")]
public record AiResolutionPlanResource(
    [SwaggerParameter(Description = "AI resolution plan identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Incident identifier")]
    int IncidentId,
    [SwaggerParameter(Description = "Review lifecycle status")]
    string Status,
    [SwaggerParameter(Description = "Generated plan summary")]
    string Summary,
    [SwaggerParameter(Description = "Generated probable cause")]
    string ProbableCause,
    [SwaggerParameter(Description = "Ordered recommended steps")]
    IReadOnlyCollection<AiResolutionPlanStepResource> RecommendedSteps,
    [SwaggerParameter(Description = "Generated corrective action draft")]
    string CorrectiveActionDraft,
    [SwaggerParameter(Description = "Generated resolution notes draft")]
    string ResolutionNotesDraft,
    [SwaggerParameter(Description = "Whether escalation is recommended")]
    bool EscalationRecommended,
    [SwaggerParameter(Description = "Escalation urgency")]
    string EscalationUrgency,
    [SwaggerParameter(Description = "Escalation reason")]
    string EscalationReason,
    [SwaggerParameter(Description = "Evidence required before closing the incident")]
    IReadOnlyCollection<string> RequiredEvidence,
    [SwaggerParameter(Description = "Uncertainty notes attached by the model")]
    IReadOnlyCollection<string> UncertaintyNotes,
    [SwaggerParameter(Description = "AI provider identifier")]
    string ModelProvider,
    [SwaggerParameter(Description = "AI model identifier")]
    string ModelName,
    [SwaggerParameter(Description = "Provider metadata captured for audit")]
    string? ProviderMetadata,
    [SwaggerParameter(Description = "Generation timestamp")]
    DateTimeOffset GeneratedAt,
    [SwaggerParameter(Description = "Approval timestamp")]
    DateTimeOffset? ApprovedAt,
    [SwaggerParameter(Description = "Approving actor")]
    string? ApprovedBy,
    [SwaggerParameter(Description = "Rejection timestamp")]
    DateTimeOffset? RejectedAt,
    [SwaggerParameter(Description = "Rejecting actor")]
    string? RejectedBy,
    [SwaggerParameter(Description = "Rejection reason")]
    string? RejectionReason,
    [SwaggerParameter(Description = "Final approved corrective action")]
    string? FinalCorrectiveAction,
    [SwaggerParameter(Description = "Final approved resolution notes")]
    string? FinalResolutionNotes);
