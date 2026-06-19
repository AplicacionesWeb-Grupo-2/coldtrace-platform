using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a maintenance schedule REST resource from a domain entity.
/// </summary>
public static class MaintenanceScheduleResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="MaintenanceSchedule" /> into a <see cref="MaintenanceScheduleResource" />.
    /// </summary>
    public static MaintenanceScheduleResource ToResourceFromEntity(MaintenanceSchedule schedule) =>
        new(schedule.Id,
            schedule.OrganizationId,
            schedule.AssetId,
            schedule.Uuid,
            schedule.ScheduledDate,
            schedule.FrequencyDays,
            schedule.ResponsibleUserId,
            schedule.Observations,
            schedule.Status,
            schedule.CreatedAt,
            schedule.UpdatedAt);
}
