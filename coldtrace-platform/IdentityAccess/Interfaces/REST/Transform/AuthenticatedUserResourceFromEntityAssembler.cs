using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an authenticated user resource from a user and JWT.
/// </summary>
public static class AuthenticatedUserResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a user and token into an authenticated user response resource.
    /// </summary>
    /// <param name="user">Authenticated user.</param>
    /// <param name="token">Issued JWT.</param>
    /// <returns>An authenticated user response resource.</returns>
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string token) =>
        new(
            user.Id,
            user.Uuid ?? $"USR-{user.Id}",
            user.OrganizationUserId ?? user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.OrganizationId,
            user.RoleId,
            token);
}
