using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an UpdateAssetCommand from an UpdateAssetResource.
/// </summary>
public static class UpdateAssetCommandFromResourceAssembler
{
    public static UpdateAssetCommand ToCommandFromResource(
        UpdateAssetResource resource,
        int organizationId,
        int assetId) =>
        new(organizationId, assetId, resource.LocationId, resource.Uuid, resource.Type,
            resource.Name, resource.Capacity, resource.Description, resource.Status);
}