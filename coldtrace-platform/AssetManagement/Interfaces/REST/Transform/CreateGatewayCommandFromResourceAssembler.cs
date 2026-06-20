using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a create gateway command from a REST resource.
/// </summary>
public static class CreateGatewayCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create gateway request into a command.
    /// </summary>
    /// <param name="resource">Create gateway request resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <returns>A create gateway command.</returns>
    public static CreateGatewayCommand ToCommandFromResource(
        CreateGatewayResource resource,
        int organizationId) =>
        new(
            organizationId,
            resource.LocationId,
            resource.Uuid,
            resource.Name,
            resource.Network,
            resource.Status);
}
