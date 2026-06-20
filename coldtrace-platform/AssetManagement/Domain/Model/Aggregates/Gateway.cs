using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Gateway aggregate for the asset management context.
/// </summary>
public class Gateway : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Gateway()
    {
        Uuid = string.Empty;
        Name = string.Empty;
        Network = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates a gateway from a create command.
    /// </summary>
    /// <param name="command">Command containing gateway data.</param>
    public Gateway(CreateGatewayCommand command)
    {
        OrganizationId = command.OrganizationId;
        LocationId = command.LocationId;
        Uuid = command.Uuid;
        Name = command.Name;
        Network = command.Network;
        Status = command.Status;
    }

    /// <summary>
    ///     Gets the server-generated gateway identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the installation location identifier.
    /// </summary>
    public int LocationId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the installation location.
    /// </summary>
    public Location Location { get; private set; } = null!;

    /// <summary>
    ///     Gets the gateway unique identifier.
    /// </summary>
    public string Uuid { get; private set; }

    /// <summary>
    ///     Gets the gateway name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the gateway network name.
    /// </summary>
    public string Network { get; private set; }

    /// <summary>
    ///     Gets the gateway status.
    /// </summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates gateway data.
    /// </summary>
    /// <param name="command">Command containing updated data.</param>
    public void Update(UpdateGatewayCommand command)
    {
        LocationId = command.LocationId;
        Uuid = command.Uuid;
        Name = command.Name;
        Network = command.Network;
        Status = command.Status;
    }
}
