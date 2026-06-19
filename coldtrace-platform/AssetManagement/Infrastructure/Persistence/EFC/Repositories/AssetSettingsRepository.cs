using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for asset settings persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class AssetSettingsRepository(AppDbContext context)
    : BaseRepository<AssetSettings>(context), IAssetSettingsRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<AssetSettings>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AssetSettings>()
            .Where(s => s.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssetSettings?> FindByOrganizationIdAndAssetIdAsync(
        int organizationId,
        int assetId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AssetSettings>()
            .FirstOrDefaultAsync(
                s => s.OrganizationId == organizationId && s.AssetId == assetId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssetSettings?> FindDefaultByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AssetSettings>()
            .FirstOrDefaultAsync(
                s => s.OrganizationId == organizationId && s.AssetId == null,
                cancellationToken);
    }
}