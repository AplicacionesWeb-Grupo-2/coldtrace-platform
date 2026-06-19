namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization-scoped IoT device.
/// </summary>
public record CreateIotDeviceCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized IoT device data.
    /// </summary>
    public CreateIotDeviceCommand(
        int organizationId,
        int gatewayId,
        int? assetId,
        string uuid,
        string name,
        string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        GatewayId = RequirePositive(gatewayId, nameof(gatewayId));
        AssetId = RequirePositiveOrNull(assetId, nameof(assetId));
        Uuid = RequireNonBlank(uuid);
        Name = RequireNonBlank(name);
        Status = RequireNonBlank(status);
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the connected gateway identifier.
    /// </summary>
    public int GatewayId { get; init; }

    /// <summary>
    ///     Gets the optional assigned asset identifier.
    /// </summary>
    public int? AssetId { get; init; }

    /// <summary>
    ///     Gets the device unique identifier.
    /// </summary>
    public string Uuid { get; init; }

    /// <summary>
    ///     Gets the device name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     Gets the device status.
    /// </summary>
    public string Status { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int? RequirePositiveOrNull(int? value, string name)
    {
        if (value is <= 0) throw new ArgumentException($"{name} must be positive when provided.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
