using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Alerts.Application.QueryServices;

/// <summary>
///     Application service contract for AI resolution plan queries.
/// </summary>
public interface IAiResolutionPlanQueryService
{
    Task<Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>> Handle(
        GetAiResolutionPlansByIncidentIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
