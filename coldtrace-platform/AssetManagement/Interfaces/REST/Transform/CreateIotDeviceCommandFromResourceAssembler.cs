using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a create IoT device command from a REST resource.
/// </summary>
public static class CreateIotDeviceCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create IoT device request into a command.
    /// </summary>
    public static CreateIotDeviceCommand ToCommandFromResource(
        CreateIotDeviceResource resource,
        int organizationId) =>
        new(
            organizationId,
            resource.GatewayId,
            resource.AssetId,
            resource.Uuid,
            resource.DeviceType,
            resource.Model,
            resource.MeasurementType,
            resource.MeasurementParameters,
            resource.ReadingFrequencySeconds,
            resource.Status,
            resource.CalibrationStatus,
            resource.LastCalibrationDate,
            resource.NextCalibrationDate);
}
