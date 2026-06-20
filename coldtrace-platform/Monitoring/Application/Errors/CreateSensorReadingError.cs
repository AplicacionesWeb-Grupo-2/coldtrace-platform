namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors that can occur while creating a sensor reading.
/// </summary>
public enum CreateSensorReadingError
{
    OrganizationNotFound,
    AssetNotFound,
    IotDeviceNotFound,
    GatewayNotFound,
    DeviceNotAssignedToAsset,
    IncompatibleLocation,
    DeviceOffline,
    GatewayOffline,
    AssetSettingsNotFound,
    UnsupportedMeasurement,
    UnexpectedError
}
