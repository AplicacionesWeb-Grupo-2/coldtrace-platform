using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Reports.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for report persistence.
/// </summary>
public class ReportRepository(AppDbContext context) : BaseRepository<Report>(context), IReportRepository
{
    public async Task<IEnumerable<Report>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Report>()
            .Where(report => report.OrganizationId == organizationId)
            .OrderByDescending(report => report.GeneratedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Report>()
            .FirstOrDefaultAsync(
                report => report.Id == id && report.OrganizationId == organizationId,
                cancellationToken);
    }
}
