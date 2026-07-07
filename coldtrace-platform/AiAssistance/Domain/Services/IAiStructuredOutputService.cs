using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AiAssistance.Domain.Services;

/// <summary>
///     Application service contract for provider-neutral structured AI outputs.
/// </summary>
public interface IAiStructuredOutputService
{
    Task<Result<IncidentResolutionPlanOutput, AiGenerationError>> GenerateIncidentResolutionPlanAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default);

    Task<Result<ComplianceSummaryOutput, AiGenerationError>> GenerateComplianceSummaryAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default);

    Task<Result<DashboardInterpretationOutput, AiGenerationError>> GenerateDashboardInterpretationAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default);
}
