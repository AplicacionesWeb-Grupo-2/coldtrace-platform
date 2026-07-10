using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for IoT device persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class IotDeviceRepository(AppDbContext context) : BaseRepository<IotDevice>(context), IIotDeviceRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<IotDevice>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<IotDevice>()
            .Where(device => device.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IotDevice?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<IotDevice>()
            .FirstOrDefaultAsync(
                device => device.Id == id && device.OrganizationId == organizationId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default)
    {
        var normalizedUuid = uuid.Trim().ToLowerInvariant();
        return await Context.Set<IotDevice>()
            .AnyAsync(
                device => device.OrganizationId == organizationId &&
                           device.Uuid.ToLower() == normalizedUuid,
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
        return await Context.Set<IotDevice>()
            .AnyAsync(
                device => device.OrganizationId == organizationId &&
                           device.Id != id &&
                           device.Uuid.ToLower() == normalizedUuid,
                cancellationToken);
    }
}
