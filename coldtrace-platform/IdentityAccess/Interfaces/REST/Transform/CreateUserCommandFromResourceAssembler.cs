using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles a user creation command from a request resource.
/// </summary>
public static class CreateUserCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create user resource into a command.
    /// </summary>
    /// <param name="resource">The create user resource.</param>
    /// <param name="organizationId">Organization identifier from the route.</param>
    /// <returns>A create user command.</returns>
    public static CreateUserCommand ToCommandFromResource(CreateUserResource resource, int organizationId) =>
        new(
            resource.FirstName,
            resource.LastName,
            resource.Email,
            resource.Password,
            organizationId,
            resource.RoleId);
}
