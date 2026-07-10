using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a user resource from a user aggregate.
/// </summary>
public static class UserResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a user aggregate into a response resource.
    /// </summary>
    /// <param name="entity">The user aggregate.</param>
    /// <returns>A user response resource.</returns>
    public static UserResource ToResourceFromEntity(User entity) =>
        new(
            entity.Id,
            entity.Uuid ?? $"USR-{entity.Id}",
            entity.OrganizationUserId ?? entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.OrganizationId,
            entity.RoleId);
}
