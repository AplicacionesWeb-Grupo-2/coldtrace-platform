namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for updating an organization-scoped gateway.
/// </summary>
public record UpdateGatewayCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized gateway data.
    /// </summary>
    public UpdateGatewayCommand(
        int organizationId,
        int gatewayId,
        int locationId,
        string uuid,
        string name,
        string network,
        string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        GatewayId = RequirePositive(gatewayId, nameof(gatewayId));
        LocationId = RequirePositive(locationId, nameof(locationId));
        Uuid = RequireNonBlank(uuid);
        Name = RequireNonBlank(name);
        Network = RequireNonBlank(network);
        Status = RequireNonBlank(status);
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the gateway identifier.
    /// </summary>
    public int GatewayId { get; init; }

    /// <summary>
    ///     Gets the installation location identifier.
    /// </summary>
    public int LocationId { get; init; }

    /// <summary>
    ///     Gets the gateway unique identifier.
    /// </summary>
    public string Uuid { get; init; }

    /// <summary>
    ///     Gets the gateway name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     Gets the gateway network name.
    /// </summary>
    public string Network { get; init; }

    /// <summary>
    ///     Gets the gateway status.
    /// </summary>
    public string Status { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
