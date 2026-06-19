using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an IoT device resource from a domain entity.
/// </summary>
public static class IotDeviceResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts an IoT device entity into a response resource.
    /// </summary>
    public static IotDeviceResource ToResourceFromEntity(IotDevice entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.GatewayId,
            entity.AssetId,
            entity.Uuid,
            entity.Name,
            entity.Status);
}
