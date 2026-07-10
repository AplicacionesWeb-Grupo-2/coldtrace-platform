namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving organization IoT devices.
/// </summary>
public enum GetIotDevicesByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
