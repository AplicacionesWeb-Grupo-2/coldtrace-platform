using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for location persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class LocationRepository(AppDbContext context) : BaseRepository<Location>(context), ILocationRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Location>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Location>()
            .Where(location => location.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Location?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Location>()
            .FirstOrDefaultAsync(
                location => location.Id == id && location.OrganizationId == organizationId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByOrganizationIdAndNameAsync(
        int organizationId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await Context.Set<Location>()
            .AnyAsync(
                location => location.OrganizationId == organizationId &&
                            location.Name.ToLower() == normalizedName,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByOrganizationIdAndNameAndIdNotAsync(
        int organizationId,
        string name,
        int id,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await Context.Set<Location>()
            .AnyAsync(
                location => location.OrganizationId == organizationId &&
                            location.Id != id &&
                            location.Name.ToLower() == normalizedName,
                cancellationToken);
    }
}
