using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Asset aggregate for the asset management context.
/// </summary>
public class Asset : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Asset()
    {
        Uuid = string.Empty;
        Type = string.Empty;
        Name = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates an asset from a create command.
    /// </summary>
    /// <param name="command">Command containing asset data.</param>
    public Asset(CreateAssetCommand command)
    {
        OrganizationId = command.OrganizationId;
        LocationId = command.LocationId;
        Uuid = command.Uuid;
        Type = command.Type;
        Name = command.Name;
        Capacity = command.Capacity;
        Description = command.Description;
        Status = command.Status;
    }

    /// <summary>Gets the server-generated asset identifier.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; private set; }

    /// <summary>Gets the placement location identifier.</summary>
    public int LocationId { get; private set; }

    /// <summary>Gets the placement location.</summary>
    public Location Location { get; private set; } = null!;

    /// <summary>Gets the asset unique identifier inside the organization.</summary>
    public string Uuid { get; private set; }

    /// <summary>Gets the business asset type.</summary>
    public string Type { get; private set; }

    /// <summary>Gets the asset display name.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the asset capacity.</summary>
    public double Capacity { get; private set; }

    /// <summary>Gets the optional asset description.</summary>
    public string? Description { get; private set; }

    /// <summary>Gets the asset operational status.</summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates asset data.
    /// </summary>
    /// <param name="command">Command containing updated data.</param>
    public void Update(UpdateAssetCommand command)
    {
        LocationId = command.LocationId;
        Uuid = command.Uuid;
        Type = command.Type;
        Name = command.Name;
        Capacity = command.Capacity;
        Description = command.Description;
        Status = command.Status;
    }
}