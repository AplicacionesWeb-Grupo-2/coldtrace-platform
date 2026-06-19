namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors that can occur while creating a sensor reading.
/// </summary>
public enum CreateSensorReadingError
{
    OrganizationNotFound,
    IotDeviceNotFound,
    UnexpectedError
}
