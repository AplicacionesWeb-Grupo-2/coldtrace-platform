using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Alerts.Domain.Repositories;

/// <summary>
///     Incident repository contract.
/// </summary>
public interface IIncidentRepository : IBaseRepository<Incident>
{
    Task<IEnumerable<Incident>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<Incident?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);
}
