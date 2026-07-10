namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for deleting an organization-scoped location.
/// </summary>
public record DeleteLocationCommand
{
    /// <summary>
    ///     Creates a command with validated route identifiers.
    /// </summary>
    public DeleteLocationCommand(int organizationId, int locationId)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        LocationId = RequirePositive(locationId, nameof(locationId));
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the location identifier.
    /// </summary>
    public int LocationId { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(name, "Identifier must be positive.");
        return value;
    }
}
