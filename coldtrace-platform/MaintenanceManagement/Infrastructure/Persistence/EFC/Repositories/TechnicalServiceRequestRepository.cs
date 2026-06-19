using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.MaintenanceManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for technical service request persistence.
/// </summary>
public class TechnicalServiceRequestRepository(AppDbContext context)
    : BaseRepository<TechnicalServiceRequest>(context), ITechnicalServiceRequestRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<TechnicalServiceRequest>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<TechnicalServiceRequest>()
            .Where(r => r.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TechnicalServiceRequest?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<TechnicalServiceRequest>()
            .FirstOrDefaultAsync(
                r => r.Id == id && r.OrganizationId == organizationId,
                cancellationToken);
    }
}
