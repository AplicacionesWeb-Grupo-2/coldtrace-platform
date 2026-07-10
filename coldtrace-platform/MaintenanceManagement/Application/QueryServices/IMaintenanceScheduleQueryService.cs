using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.MaintenanceManagement.Application.QueryServices;

/// <summary>
///     Contract for maintenance schedule query operations.
/// </summary>
public interface IMaintenanceScheduleQueryService
{
    Task<Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>> Handle(
        GetMaintenanceSchedulesByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>> Handle(
        GetMaintenanceScheduleByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
