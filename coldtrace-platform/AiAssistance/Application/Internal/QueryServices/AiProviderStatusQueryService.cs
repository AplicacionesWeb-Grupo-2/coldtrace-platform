using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Domain.Model.Queries;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.AiAssistance.Infrastructure.Providers;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.AiAssistance.Application.Internal.QueryServices;

/// <summary>
///     Application service for AI provider configuration status queries.
/// </summary>
public class AiProviderStatusQueryService(
    IOptions<AiOptions> options,
    IAiChatClientAdapter chatClientAdapter,
    ILogger<AiProviderStatusQueryService> logger)
    : IAiProviderStatusQueryService
{
    private static readonly string[] StructuredOutputContracts =
    [
        "incident-resolution-plan",
        "compliance-summary",
        "dashboard-interpretation"
    ];

    /// <inheritdoc />
    public Task<Result<AiProviderStatus, GetAiProviderStatusError>> Handle(
        GetAiProviderStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var aiOptions = options.Value;
            var status = new AiProviderStatus(
                aiOptions.Provider,
                aiOptions.Model,
                aiOptions.Enabled,
                aiOptions.IsConfigured,
                aiOptions.HasEndpoint,
                aiOptions.HasApiKey,
                chatClientAdapter.HasClient,
                aiOptions.TimeoutSeconds,
                StructuredOutputContracts);

            return Task.FromResult<Result<AiProviderStatus, GetAiProviderStatusError>>(
                new Result<AiProviderStatus, GetAiProviderStatusError>.Success(status));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while reading AI provider status.");
            return Task.FromResult<Result<AiProviderStatus, GetAiProviderStatusError>>(
                new Result<AiProviderStatus, GetAiProviderStatusError>.Failure(
                    GetAiProviderStatusError.UnexpectedError));
        }
    }
}
