using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Alerts.Domain.Repositories;

/// <summary>
///     AI resolution plan repository contract.
/// </summary>
public interface IAiResolutionPlanRepository : IBaseRepository<AiResolutionPlan>
{
    Task<IEnumerable<AiResolutionPlan>> FindAllByIncidentIdAndOrganizationIdAsync(
        int incidentId,
        int organizationId,
        CancellationToken cancellationToken = default);
}
