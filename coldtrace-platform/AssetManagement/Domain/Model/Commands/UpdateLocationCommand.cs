namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for updating an organization-scoped location.
/// </summary>
public record UpdateLocationCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized location data.
    /// </summary>
    public UpdateLocationCommand(
        int organizationId,
        int locationId,
        string name,
        string type,
        string? address,
        string? description,
        string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        LocationId = RequirePositive(locationId, nameof(locationId));
        Name = RequireNonBlank(name);
        Type = RequireNonBlank(type);
        Address = NormalizeOptional(address);
        Description = NormalizeOptional(description);
        Status = RequireNonBlank(status);
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the location identifier.
    /// </summary>
    public int LocationId { get; init; }

    /// <summary>
    ///     Gets the location name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///     Gets the location type.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    ///     Gets the optional location address.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    ///     Gets the optional location description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Gets the location status.
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

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
