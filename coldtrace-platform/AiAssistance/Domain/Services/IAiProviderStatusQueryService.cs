using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AiAssistance.Domain.Services;

/// <summary>
///     Application service contract for AI provider status queries.
/// </summary>
public interface IAiProviderStatusQueryService
{
    Task<Result<AiProviderStatus, GetAiProviderStatusError>> Handle(
        GetAiProviderStatusQuery query,
        CancellationToken cancellationToken = default);
}
