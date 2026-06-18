namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization-scoped asset.
/// </summary>
public record CreateAssetCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized asset data.
    /// </summary>
    public CreateAssetCommand(
        int organizationId,
        int locationId,
        string uuid,
        string type,
        string name,
        double capacity,
        string? description,
        string status)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        LocationId = RequirePositive(locationId, nameof(locationId));
        Uuid = RequireNonBlank(uuid);
        Type = RequireNonBlank(type);
        Name = RequireNonBlank(name);
        Capacity = RequirePositive(capacity, nameof(capacity));
        Description = description?.Trim();
        Status = RequireNonBlank(status);
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the placement location identifier.</summary>
    public int LocationId { get; init; }

    /// <summary>Gets the asset unique identifier inside the organization.</summary>
    public string Uuid { get; init; }

    /// <summary>Gets the business asset type.</summary>
    public string Type { get; init; }

    /// <summary>Gets the asset display name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the asset capacity.</summary>
    public double Capacity { get; init; }

    /// <summary>Gets the optional asset description.</summary>
    public string? Description { get; init; }

    /// <summary>Gets the asset operational status.</summary>
    public string Status { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static double RequirePositive(double value, string name)
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