namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for deleting an organization-scoped gateway.
/// </summary>
public record DeleteGatewayCommand
{
    /// <summary>
    ///     Creates a command with validated route identifiers.
    /// </summary>
    /// <param name="organizationId">The owning organization identifier.</param>
    /// <param name="gatewayId">The gateway identifier.</param>
    public DeleteGatewayCommand(int organizationId, int gatewayId)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        GatewayId = RequirePositive(gatewayId, nameof(gatewayId));
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the gateway identifier.
    /// </summary>
    public int GatewayId { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
