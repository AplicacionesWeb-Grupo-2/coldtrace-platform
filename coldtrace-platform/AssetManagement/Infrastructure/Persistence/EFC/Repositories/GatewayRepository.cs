using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for gateway persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class GatewayRepository(AppDbContext context) : BaseRepository<Gateway>(context), IGatewayRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Gateway>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Gateway>()
            .Where(gateway => gateway.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Gateway?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Gateway>()
            .FirstOrDefaultAsync(
                gateway => gateway.Id == id && gateway.OrganizationId == organizationId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default)
    {
        var normalizedUuid = uuid.Trim().ToLowerInvariant();
        return await Context.Set<Gateway>()
            .AnyAsync(
                gateway => gateway.OrganizationId == organizationId &&
                           gateway.Uuid.ToLower() == normalizedUuid,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByOrganizationIdAndUuidAndIdNotAsync(
        int organizationId,
        string uuid,
        int id,
        CancellationToken cancellationToken = default)
    {
        var normalizedUuid = uuid.Trim().ToLowerInvariant();
        return await Context.Set<Gateway>()
            .AnyAsync(
                gateway => gateway.OrganizationId == organizationId &&
                           gateway.Id != id &&
                           gateway.Uuid.ToLower() == normalizedUuid,
                cancellationToken);
    }
}
