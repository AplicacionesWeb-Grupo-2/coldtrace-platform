using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Reports.Domain.Repositories;

/// <summary>
///     Report repository contract.
/// </summary>
public interface IReportRepository : IBaseRepository<Report>
{
    Task<IEnumerable<Report>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<Report?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);
}
