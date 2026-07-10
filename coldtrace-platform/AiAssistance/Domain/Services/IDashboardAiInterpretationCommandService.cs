using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AiAssistance.Domain.Services;

/// <summary>
///     Application command service for dashboard AI interpretation use cases.
/// </summary>
public interface IDashboardAiInterpretationCommandService
{
    Task<Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>> Handle(
        GenerateDashboardAiInterpretationCommand command,
        CancellationToken cancellationToken = default);
}
