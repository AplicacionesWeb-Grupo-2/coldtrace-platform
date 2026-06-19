namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving sensor readings for an organization.
/// </summary>
public enum GetSensorReadingsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
