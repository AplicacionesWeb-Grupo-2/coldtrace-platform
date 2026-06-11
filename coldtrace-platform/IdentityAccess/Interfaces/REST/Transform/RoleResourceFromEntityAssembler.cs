using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles role resources from role aggregates.
/// </summary>
public static class RoleResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a role aggregate into a response resource.
    /// </summary>
    /// <param name="entity">The role aggregate.</param>
    /// <returns>A role response resource.</returns>
    public static RoleResource ToResourceFromEntity(Role entity) =>
        new(
            entity.Id,
            entity.Name,
            entity.Label,
            entity.Permissions
                .OrderBy(permission => permission.Id)
                .Select(ToPermissionResourceFromValueObject));

    private static PermissionResource ToPermissionResourceFromValueObject(Permission permission) =>
        new(permission.Id, permission.Resource, permission.Action, permission.Description);
}
