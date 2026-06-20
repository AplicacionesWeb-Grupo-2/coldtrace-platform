namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors that can occur while creating an IoT device.
/// </summary>
public enum CreateIotDeviceError
{
    DuplicateUuid,
    OrganizationNotFound,
    GatewayNotFound,
    AssetNotFound,
    AssetLocationNotCompatible,
    UnexpectedError
}
