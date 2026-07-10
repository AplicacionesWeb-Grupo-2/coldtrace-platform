using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for asset persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class AssetRepository(AppDbContext context) : BaseRepository<Asset>(context), IAssetRepository
{
    public async Task<IEnumerable<Asset>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Asset>()
            .Where(asset => asset.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Asset?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Asset>()
            .FirstOrDefaultAsync(
                asset => asset.Id == id && asset.OrganizationId == organizationId,
                cancellationToken);
    }
    
    public async Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default)
    {
        var normalizedUuid = uuid.Trim().ToLowerInvariant();
        return await Context.Set<Asset>()
            .AnyAsync(
                asset => asset.OrganizationId == organizationId &&
                         asset.Uuid.ToLower() == normalizedUuid,
                cancellationToken);
    }
    
    public async Task<bool> ExistsByOrganizationIdAndUuidAndIdNotAsync(
        int organizationId,
        string uuid,
        int id,
        CancellationToken cancellationToken = default)
    {
        var normalizedUuid = uuid.Trim().ToLowerInvariant();
        return await Context.Set<Asset>()
            .AnyAsync(
                asset => asset.OrganizationId == organizationId &&
                         asset.Id != id &&
                         asset.Uuid.ToLower() == normalizedUuid,
                cancellationToken);
    }
}