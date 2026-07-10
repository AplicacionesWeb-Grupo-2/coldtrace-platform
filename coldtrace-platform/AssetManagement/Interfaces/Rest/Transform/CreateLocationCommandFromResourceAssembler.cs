using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a create location command from a REST resource.
/// </summary>
public static class CreateLocationCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create location request into a command.
    /// </summary>
    /// <param name="resource">Create location request resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <returns>A create location command.</returns>
    public static CreateLocationCommand ToCommandFromResource(
        CreateLocationResource resource,
        int organizationId) =>
        new(
            organizationId,
            resource.Name,
            resource.Type,
            resource.Address,
            resource.Description,
            resource.Status);
}
