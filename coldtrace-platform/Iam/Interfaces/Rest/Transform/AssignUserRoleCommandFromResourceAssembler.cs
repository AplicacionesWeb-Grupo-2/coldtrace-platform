using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

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
