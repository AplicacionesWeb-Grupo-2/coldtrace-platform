using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an update IoT device command from a REST resource.
/// </summary>
public static class UpdateIotDeviceCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an update IoT device request into a command.
    /// </summary>
    public static UpdateIotDeviceCommand ToCommandFromResource(
        UpdateIotDeviceResource resource,
        int organizationId,
        int iotDeviceId) =>
        new(
            organizationId,
            iotDeviceId,
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
