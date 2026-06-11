using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Location aggregate for the asset management context.
/// </summary>
public class Location : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Location()
    {
        Name = string.Empty;
        Type = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates a location from a create command.
    /// </summary>
    /// <param name="command">Command containing location data.</param>
    public Location(CreateLocationCommand command)
    {
        OrganizationId = command.OrganizationId;
        Name = command.Name;
        Type = command.Type;
        Address = command.Address;
        Description = command.Description;
        Status = command.Status;
    }

    /// <summary>
    ///     Gets the server-generated location identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the location name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the location type.
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    ///     Gets the optional location address.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    ///     Gets the optional location description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    ///     Gets the location status.
    /// </summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates location data.
    /// </summary>
    /// <param name="command">Command containing updated data.</param>
    public void Update(UpdateLocationCommand command)
    {
        Name = command.Name;
        Type = command.Type;
        Address = command.Address;
        Description = command.Description;
        Status = command.Status;
    }
}
