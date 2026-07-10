namespace ColdTrace.Platform.Monitoring.Domain.Model.Errors;

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
