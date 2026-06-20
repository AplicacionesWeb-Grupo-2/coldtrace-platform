namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving one sensor reading.
/// </summary>
public enum GetSensorReadingByIdAndOrganizationError
{
    OrganizationNotFound,
    SensorReadingNotFound,
    UnexpectedError
}
