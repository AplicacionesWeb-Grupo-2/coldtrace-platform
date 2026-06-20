using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an update gateway command from a REST resource.
/// </summary>
public static class UpdateGatewayCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an update gateway request into a command.
    /// </summary>
    /// <param name="resource">Update gateway request resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <param name="gatewayId">Gateway identifier from the route.</param>
    /// <returns>An update gateway command.</returns>
    public static UpdateGatewayCommand ToCommandFromResource(
        UpdateGatewayResource resource,
        int organizationId,
        int gatewayId) =>
        new(
            organizationId,
            gatewayId,
            resource.LocationId,
            resource.Uuid,
            resource.Name,
            resource.Network,
            resource.Status);
}
