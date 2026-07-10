using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AiAssistance.Application.CommandServices;

/// <summary>
///     Application command service for dashboard AI interpretation use cases.
/// </summary>
public interface IDashboardAiInterpretationCommandService
{
    Task<Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>> Handle(
        GenerateDashboardAiInterpretationCommand command,
        CancellationToken cancellationToken = default);
}
