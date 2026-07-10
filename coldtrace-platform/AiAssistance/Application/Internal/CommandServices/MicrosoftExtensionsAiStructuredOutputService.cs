using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Application.CommandServices;
using ColdTrace.Platform.AiAssistance.Application.QueryServices;
using ColdTrace.Platform.AiAssistance.Application.Internal.OutboundServices;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.AiAssistance.Infrastructure.Providers;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.AiAssistance.Application.Internal.CommandServices;

/// <summary>
///     Structured output service backed by Microsoft.Extensions.AI.
/// </summary>
public class MicrosoftExtensionsAiStructuredOutputService(
    IAiChatClientAdapter chatClientAdapter,
    IOptions<AiOptions> options,
    ILogger<MicrosoftExtensionsAiStructuredOutputService> logger)
    : IAiStructuredOutputService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    public Task<Result<IncidentResolutionPlanOutput, AiGenerationError>> GenerateIncidentResolutionPlanAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        GenerateStructuredOutputAsync<IncidentResolutionPlanOutput>(
            prompt,
            "incident resolution plan",
            cancellationToken);

    public Task<Result<ComplianceSummaryOutput, AiGenerationError>> GenerateComplianceSummaryAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        GenerateStructuredOutputAsync<ComplianceSummaryOutput>(
            prompt,
            "compliance summary",
            cancellationToken);

    public Task<Result<DashboardInterpretationOutput, AiGenerationError>> GenerateDashboardInterpretationAsync(
        AiStructuredPrompt prompt,
        CancellationToken cancellationToken = default) =>
        GenerateStructuredOutputAsync<DashboardInterpretationOutput>(
            prompt,
            "dashboard interpretation",
            cancellationToken);

    private async Task<Result<TOutput, AiGenerationError>> GenerateStructuredOutputAsync<TOutput>(
        AiStructuredPrompt prompt,
        string contractName,
        CancellationToken cancellationToken)
        where TOutput : class
    {
        var aiOptions = options.Value;
        if (!aiOptions.Enabled)
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.ProviderDisabled);

        var chatClient = chatClientAdapter.GetClient();
        if (chatClient is null)
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.ProviderNotConfigured);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(aiOptions.TimeoutSeconds));

        try
        {
            var response = await chatClient.GetResponseAsync<TOutput>(
                BuildMessages(prompt),
                SerializerOptions,
                new ChatOptions { ModelId = aiOptions.Model },
                useJsonSchemaResponseFormat: true,
                cancellationToken: timeout.Token);

            if (response.TryGetResult(out var output) && output is not null)
                return new Result<TOutput, AiGenerationError>.Success(output);

            logger.LogWarning(
                "AI provider returned invalid structured output. Contract: {ContractName}",
                contractName);
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.InvalidStructuredOutput);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "AI structured output request timed out after {TimeoutSeconds} seconds. Contract: {ContractName}",
                aiOptions.TimeoutSeconds,
                contractName);
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.ProviderTimeout);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "AI structured output could not be parsed. Contract: {ContractName}", contractName);
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.InvalidStructuredOutput);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI provider request failed. Contract: {ContractName}", contractName);
            return new Result<TOutput, AiGenerationError>.Failure(AiGenerationError.ProviderUnavailable);
        }
    }

    private static IReadOnlyList<ChatMessage> BuildMessages(AiStructuredPrompt prompt) =>
        [
            new(ChatRole.System, prompt.SystemInstruction),
            new(ChatRole.User, BuildUserMessage(prompt))
        ];

    private static string BuildUserMessage(AiStructuredPrompt prompt)
    {
        if (prompt.Context is null || prompt.Context.Count == 0) return prompt.UserRequest;

        var builder = new StringBuilder(prompt.UserRequest);
        builder.AppendLine();
        builder.AppendLine("Context:");
        foreach (var (key, value) in prompt.Context)
        {
            builder.Append(key);
            builder.Append(": ");
            builder.AppendLine(value);
        }

        return builder.ToString();
    }
}
