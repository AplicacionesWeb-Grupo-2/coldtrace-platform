namespace ColdTrace.Platform.Monitoring.Application.Errors;

/// <summary>
///     Errors produced while generating demo sensor readings.
/// </summary>
public enum GenerateDemoSensorReadingsError
{
    OrganizationNotFound,
    AssetNotFound,
    NoEligibleDevices,
    UnexpectedError
}
