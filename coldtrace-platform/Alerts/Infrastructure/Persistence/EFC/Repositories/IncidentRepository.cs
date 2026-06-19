using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Alerts.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for incident persistence.
/// </summary>
public class IncidentRepository(AppDbContext context) : BaseRepository<Incident>(context), IIncidentRepository
{
    public async Task<IEnumerable<Incident>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Incident>()
            .Where(incident => incident.OrganizationId == organizationId)
            .OrderByDescending(incident => incident.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Incident?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Incident>()
            .FirstOrDefaultAsync(
                incident => incident.Id == id && incident.OrganizationId == organizationId,
                cancellationToken);
    }

    public async Task<bool> ExistsByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Incident>()
            .AnyAsync(
                incident => incident.Id == id && incident.OrganizationId == organizationId,
                cancellationToken);
    }
}
