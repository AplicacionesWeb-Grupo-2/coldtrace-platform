using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a create maintenance schedule command from a REST resource.
/// </summary>
public static class CreateMaintenanceScheduleCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateMaintenanceScheduleResource" /> into a
    ///     <see cref="CreateMaintenanceScheduleCommand" />.
    /// </summary>
    public static CreateMaintenanceScheduleCommand ToCommandFromResource(
        CreateMaintenanceScheduleResource resource,
        int organizationId) =>
        new(organizationId,
            resource.AssetId,
            resource.ScheduledDate,
            resource.FrequencyDays,
            resource.ResponsibleUserId,
            resource.Observations,
            resource.Status);
}
