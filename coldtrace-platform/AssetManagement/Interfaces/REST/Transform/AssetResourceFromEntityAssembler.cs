using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles asset resources from asset entities.
/// </summary>
public static class AssetResourceFromEntityAssembler
{
    public static AssetResource ToResourceFromEntity(Asset entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.LocationId,
            entity.Uuid,
            entity.Type,
            entity.Name,
            entity.Capacity,
            entity.Description,
            entity.Status);
}