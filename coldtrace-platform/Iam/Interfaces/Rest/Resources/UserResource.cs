using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a user.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Uuid">Generated user code.</param>
/// <param name="OrganizationUserId">Generated organization user identifier.</param>
/// <param name="FirstName">User first name.</param>
/// <param name="LastName">User last name.</param>
/// <param name="Email">User email address.</param>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="RoleId">Role identifier.</param>
[SwaggerSchema(Description = "A user resource")]
public record UserResource(
    [SwaggerParameter(Description = "User identifier")]
    int Id,
    [SwaggerParameter(Description = "Generated user code")]
    string Uuid,
    [SwaggerParameter(Description = "Generated organization user identifier")]
    int OrganizationUserId,
    [SwaggerParameter(Description = "User first name")]
    string FirstName,
    [SwaggerParameter(Description = "User last name")]
    string LastName,
    [SwaggerParameter(Description = "User email address")]
    string Email,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Role identifier")]
    int RoleId);
