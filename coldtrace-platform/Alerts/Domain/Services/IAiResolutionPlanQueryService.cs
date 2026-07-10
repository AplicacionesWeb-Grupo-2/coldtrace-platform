using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Alerts.Domain.Services;

/// <summary>
///     Application service contract for AI resolution plan queries.
/// </summary>
public interface IAiResolutionPlanQueryService
{
    Task<Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>> Handle(
        GetAiResolutionPlansByIncidentIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
