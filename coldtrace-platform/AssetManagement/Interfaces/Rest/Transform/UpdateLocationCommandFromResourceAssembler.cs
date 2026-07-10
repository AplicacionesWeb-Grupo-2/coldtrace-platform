using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an update location command from a REST resource.
/// </summary>
public static class UpdateLocationCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an update location request into a command.
    /// </summary>
    /// <param name="resource">Update location request resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <param name="locationId">Location identifier from the route.</param>
    /// <returns>An update location command.</returns>
    public static UpdateLocationCommand ToCommandFromResource(
        UpdateLocationResource resource,
        int organizationId,
        int locationId) =>
        new(
            organizationId,
            locationId,
            resource.Name,
            resource.Type,
            resource.Address,
            resource.Description,
            resource.Status);
}
