using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Monitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for sensor reading persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SensorReadingRepository(AppDbContext context) : BaseRepository<SensorReading>(context), ISensorReadingRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SensorReading>> FindAllByOrganizationIdAsync(
        int organizationId,
        int? assetId = null,
        int? iotDeviceId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<SensorReading>()
            .Where(reading => reading.OrganizationId == organizationId);

        if (assetId is not null) query = query.Where(reading => reading.AssetId == assetId);
        if (iotDeviceId is not null) query = query.Where(reading => reading.IotDeviceId == iotDeviceId);
        if (from is not null) query = query.Where(reading => reading.RecordedAt >= from);
        if (to is not null) query = query.Where(reading => reading.RecordedAt <= to);

        return await query.OrderByDescending(reading => reading.RecordedAt)
            .ThenByDescending(reading => reading.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SensorReading>> FindAllByOrganizationIdAndIotDeviceIdAsync(
        int organizationId,
        int iotDeviceId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SensorReading>()
            .Where(reading => reading.OrganizationId == organizationId && reading.IotDeviceId == iotDeviceId)
            .OrderByDescending(reading => reading.RecordedAt)
            .ThenByDescending(reading => reading.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SensorReading?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SensorReading>()
            .FirstOrDefaultAsync(
                reading => reading.Id == id && reading.OrganizationId == organizationId,
                cancellationToken);
    }
}
