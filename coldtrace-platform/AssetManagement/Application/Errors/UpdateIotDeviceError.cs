namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while updating an IoT device.
/// </summary>
public enum UpdateIotDeviceError
{
    DuplicateUuid,
    OrganizationNotFound,
    GatewayNotFound,
    IotDeviceNotFound,
    AssetNotFound,
    AssetLocationNotCompatible,
    UnexpectedError
}
