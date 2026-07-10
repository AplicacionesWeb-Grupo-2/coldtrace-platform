using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.MaintenanceManagement.Application.CommandServices;

/// <summary>
///     Contract for maintenance schedule command operations.
/// </summary>
public interface IMaintenanceScheduleCommandService
{
    Task<Result<MaintenanceSchedule, CreateMaintenanceScheduleError>> Handle(
        CreateMaintenanceScheduleCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>> Handle(
        UpdateMaintenanceScheduleStatusCommand command,
        CancellationToken cancellationToken = default);
}
