using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Services;

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
