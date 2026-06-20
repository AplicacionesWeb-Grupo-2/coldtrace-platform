using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.MaintenanceManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for maintenance schedule persistence.
/// </summary>
public class MaintenanceScheduleRepository(AppDbContext context)
    : BaseRepository<MaintenanceSchedule>(context), IMaintenanceScheduleRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceSchedule>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MaintenanceSchedule>()
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MaintenanceSchedule?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MaintenanceSchedule>()
            .FirstOrDefaultAsync(
                s => s.Id == id && s.OrganizationId == organizationId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasActiveScheduleForAssetAsync(
        int organizationId,
        int assetId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MaintenanceSchedule>()
            .AnyAsync(
                s => s.OrganizationId == organizationId &&
                     s.AssetId == assetId &&
                     s.Status != "completed" &&
                     s.Status != "canceled",
                cancellationToken);
    }
}
