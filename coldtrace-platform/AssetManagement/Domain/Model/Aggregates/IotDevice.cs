using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     IoT device aggregate for the asset management context.
/// </summary>
public class IotDevice : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected IotDevice()
    {
        Uuid = string.Empty;
        Name = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates an IoT device from a create command.
    /// </summary>
    /// <param name="command">Command containing device data.</param>
    public IotDevice(CreateIotDeviceCommand command)
    {
        OrganizationId = command.OrganizationId;
        GatewayId = command.GatewayId;
        AssetId = command.AssetId;
        Uuid = command.Uuid;
        Name = command.Name;
        Status = command.Status;
    }

    /// <summary>
    ///     Gets the server-generated device identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the connected gateway identifier.
    /// </summary>
    public int GatewayId { get; private set; }

    /// <summary>
    ///     Gets the optional assigned asset identifier.
    /// </summary>
    public int? AssetId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the connected gateway.
    /// </summary>
    public Gateway Gateway { get; private set; } = null!;

    /// <summary>
    ///     Gets the assigned asset.
    /// </summary>
    public Asset? Asset { get; private set; }

    /// <summary>
    ///     Gets the device unique identifier.
    /// </summary>
    public string Uuid { get; private set; }

    /// <summary>
    ///     Gets the device name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the device status.
    /// </summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Updates device data.
    /// </summary>
    /// <param name="command">Command containing updated data.</param>
    public void Update(UpdateIotDeviceCommand command)
    {
        GatewayId = command.GatewayId;
        AssetId = command.AssetId;
        Uuid = command.Uuid;
        Name = command.Name;
        Status = command.Status;
    }
}
