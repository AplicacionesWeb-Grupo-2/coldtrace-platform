using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AiAssistance.Application.Internal.CommandServices;

/// <summary>
///     Default structured output service used until a concrete provider adapter is configured.
/// </summary>
public class DisabledAiStructuredOutputService(ILogger<DisabledAiStructuredOutputService> logger)
    : IAiStructuredOutputService
{
    /// <inheritdoc />
    public Task<Result<IncidentResolutionPlanOutput, AiGenerationError>> GenerateIncidentResolutionPlanAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        ProviderNotConfigured<IncidentResolutionPlanOutput>("incident resolution plan");

    /// <inheritdoc />
    public Task<Result<ComplianceSummaryOutput, AiGenerationError>> GenerateComplianceSummaryAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        ProviderNotConfigured<ComplianceSummaryOutput>("compliance summary");

    /// <inheritdoc />
    public Task<Result<DashboardInterpretationOutput, AiGenerationError>> GenerateDashboardInterpretationAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        ProviderNotConfigured<DashboardInterpretationOutput>("dashboard interpretation");

    private Task<Result<TOutput, AiGenerationError>> ProviderNotConfigured<TOutput>(string contractName)
    {
        logger.LogInformation(
            "AI structured output request skipped because no provider adapter is configured. Contract: {ContractName}",
            contractName);

        return Task.FromResult<Result<TOutput, AiGenerationError>>(
            new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.ProviderNotConfigured));
    }
}
