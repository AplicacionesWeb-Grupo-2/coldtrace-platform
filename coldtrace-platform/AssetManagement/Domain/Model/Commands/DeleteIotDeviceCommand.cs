namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for deleting an IoT device inside an organization.
/// </summary>
public record DeleteIotDeviceCommand
{
    /// <summary>
    ///     Creates a command with validated route identifiers.
    /// </summary>
    public DeleteIotDeviceCommand(int organizationId, int iotDeviceId)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IotDeviceId = RequirePositive(iotDeviceId, nameof(iotDeviceId));
    }

    /// <summary>
    ///     Gets the organization identifier that scopes the IoT device.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the IoT device identifier to delete.
    /// </summary>
    public int IotDeviceId { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
