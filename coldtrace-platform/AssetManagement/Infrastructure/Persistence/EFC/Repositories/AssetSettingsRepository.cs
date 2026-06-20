using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for asset settings persistence.
/// </summary>
public class AssetSettingsRepository(AppDbContext context)
    : BaseRepository<AssetSettings>(context), IAssetSettingsRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<AssetSettings>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AssetSettings>()
            .Where(settings => settings.OrganizationId == organizationId)
            .OrderBy(settings => settings.AssetId.HasValue)
            .ThenBy(settings => settings.AssetId)
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
                settings => settings.OrganizationId == organizationId && settings.AssetId == assetId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssetSettings?> FindDefaultByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AssetSettings>()
            .FirstOrDefaultAsync(
                settings => settings.OrganizationId == organizationId && settings.AssetId == null,
                cancellationToken);
    }
}
