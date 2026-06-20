using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles location resources from location entities.
/// </summary>
public static class LocationResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a location aggregate to a response resource.
    /// </summary>
    /// <param name="entity">Location aggregate.</param>
    /// <returns>Location response resource.</returns>
    public static LocationResource ToResourceFromEntity(Location entity) =>
        new(
            entity.Id,
            entity.OrganizationId,
            entity.Name,
            entity.Type,
            entity.Address,
            entity.Description,
            entity.Status);
}
