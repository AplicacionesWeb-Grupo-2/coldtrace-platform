using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AiAssistance.Application.QueryServices;

/// <summary>
///     Application service contract for AI provider status queries.
/// </summary>
public interface IAiProviderStatusQueryService
{
    Task<Result<AiProviderStatus, GetAiProviderStatusError>> Handle(
        GetAiProviderStatusQuery query,
        CancellationToken cancellationToken = default);
}
