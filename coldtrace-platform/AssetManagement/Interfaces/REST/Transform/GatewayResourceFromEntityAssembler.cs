using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles gateway resources from gateway entities.
/// </summary>
public static class GatewayResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a gateway aggregate to a response resource.
    /// </summary>
    /// <param name="entity">Gateway aggregate.</param>
    /// <returns>Gateway response resource.</returns>
    public static GatewayResource ToResourceFromEntity(Gateway entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.LocationId,
            entity.Uuid,
            entity.Name,
            entity.Network,
            entity.Status);
}
