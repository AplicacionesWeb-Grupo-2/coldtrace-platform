namespace ColdTrace.Platform.Monitoring.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving one sensor reading.
/// </summary>
public enum GetSensorReadingByIdAndOrganizationError
{
    OrganizationNotFound,
    SensorReadingNotFound,
    UnexpectedError
}
