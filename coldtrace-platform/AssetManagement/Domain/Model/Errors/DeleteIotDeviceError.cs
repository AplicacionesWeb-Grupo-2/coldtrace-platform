namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while deleting an IoT device.
/// </summary>
public enum DeleteIotDeviceError
{
    /// <summary>
    ///     The owning organization was not found.
    /// </summary>
    OrganizationNotFound,

    /// <summary>
    ///     The IoT device was not found in the organization.
    /// </summary>
    IotDeviceNotFound,

    /// <summary>
    ///     Historical or otherwise dependent records prevent deletion.
    /// </summary>
    DeleteBlocked,

    /// <summary>
    ///     An unexpected persistence or application error occurred.
    /// </summary>
    UnexpectedError
}
