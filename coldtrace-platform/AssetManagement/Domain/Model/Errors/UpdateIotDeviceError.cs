namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

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
