using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Alerts.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for AI resolution plan persistence.
/// </summary>
public class AiResolutionPlanRepository(AppDbContext context)
    : BaseRepository<AiResolutionPlan>(context), IAiResolutionPlanRepository
{
    public async Task<IEnumerable<AiResolutionPlan>> FindAllByIncidentIdAndOrganizationIdAsync(
        int incidentId,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AiResolutionPlan>()
            .Where(plan => plan.IncidentId == incidentId && plan.OrganizationId == organizationId)
            .OrderByDescending(plan => plan.GeneratedAt)
            .ThenByDescending(plan => plan.Id)
            .ToListAsync(cancellationToken);
    }
}
