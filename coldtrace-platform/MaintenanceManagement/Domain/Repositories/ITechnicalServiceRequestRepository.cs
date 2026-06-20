using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;

/// <summary>
///     Technical service request repository contract.
/// </summary>
public interface ITechnicalServiceRequestRepository : IBaseRepository<TechnicalServiceRequest>
{
    Task<IEnumerable<TechnicalServiceRequest>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<TechnicalServiceRequest?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);
}
