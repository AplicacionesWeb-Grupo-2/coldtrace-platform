using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

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
