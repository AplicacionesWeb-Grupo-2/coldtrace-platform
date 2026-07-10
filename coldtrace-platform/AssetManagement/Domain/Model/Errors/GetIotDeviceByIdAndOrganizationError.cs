namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving one IoT device.
/// </summary>
public enum GetIotDeviceByIdAndOrganizationError
{
    OrganizationNotFound,
    IotDeviceNotFound,
    UnexpectedError
}
