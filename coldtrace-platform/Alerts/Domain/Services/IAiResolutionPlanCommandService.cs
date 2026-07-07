using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Alerts.Domain.Services;

/// <summary>
///     Application service contract for AI resolution plan commands.
/// </summary>
public interface IAiResolutionPlanCommandService
{
    Task<Result<AiResolutionPlan, GenerateAiResolutionPlanError>> Handle(
        GenerateAiResolutionPlanCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<AiResolutionPlan, ApproveAiResolutionPlanError>> Handle(
        ApproveAiResolutionPlanCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<AiResolutionPlan, RejectAiResolutionPlanError>> Handle(
        RejectAiResolutionPlanCommand command,
        CancellationToken cancellationToken = default);
}
