using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an assign user role command from a REST resource.
/// </summary>
public static class AssignUserRoleCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an assign user role resource into a command.
    /// </summary>
    /// <param name="resource">Role assignment request resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <param name="userId">User identifier from the route.</param>
    /// <returns>An assign user role command.</returns>
    public static AssignUserRoleCommand ToCommandFromResource(
        AssignUserRoleResource resource,
        int organizationId,
        int userId) =>
        new(organizationId, userId, resource.RoleId);
}
