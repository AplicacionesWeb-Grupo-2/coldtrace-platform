using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;

/// <summary>
///     Maintenance schedule repository contract.
/// </summary>
public interface IMaintenanceScheduleRepository : IBaseRepository<MaintenanceSchedule>
{
    Task<IEnumerable<MaintenanceSchedule>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<MaintenanceSchedule?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveScheduleForAssetAsync(
        int organizationId,
        int assetId,
        CancellationToken cancellationToken = default);
}
