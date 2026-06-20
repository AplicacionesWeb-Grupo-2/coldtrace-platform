using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an update maintenance schedule status command from a REST resource.
/// </summary>
public static class UpdateMaintenanceScheduleStatusCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="UpdateMaintenanceScheduleStatusResource" /> into a
    ///     <see cref="UpdateMaintenanceScheduleStatusCommand" />.
    /// </summary>
    public static UpdateMaintenanceScheduleStatusCommand ToCommandFromResource(
        UpdateMaintenanceScheduleStatusResource resource,
        int organizationId,
        int maintenanceScheduleId) =>
        new(organizationId, maintenanceScheduleId, resource.Status);
}
