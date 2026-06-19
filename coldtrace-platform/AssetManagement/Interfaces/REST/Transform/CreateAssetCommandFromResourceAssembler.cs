using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a CreateAssetCommand from a CreateAssetResource.
/// </summary>
public static class CreateAssetCommandFromResourceAssembler
{
    public static CreateAssetCommand ToCommandFromResource(CreateAssetResource resource, int organizationId) =>
        new(organizationId, resource.LocationId, resource.Uuid, resource.Type,
            resource.Name, resource.Capacity, resource.Description, resource.Status);
}