namespace ColdTrace.Platform.Monitoring.Domain.Model.Errors;

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
