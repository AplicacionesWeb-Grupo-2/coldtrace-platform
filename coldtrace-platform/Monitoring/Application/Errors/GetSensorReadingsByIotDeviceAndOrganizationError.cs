namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving sensor readings for one IoT device.
/// </summary>
public enum GetSensorReadingsByIotDeviceAndOrganizationError
{
    OrganizationNotFound,
    IotDeviceNotFound,
    UnexpectedError
}
