using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Monitoring.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for sensor reading persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SensorReadingRepository(AppDbContext context) : BaseRepository<SensorReading>(context), ISensorReadingRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SensorReading>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SensorReading>()
            .Where(reading => reading.OrganizationId == organizationId)
            .OrderByDescending(reading => reading.RecordedAt)
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
