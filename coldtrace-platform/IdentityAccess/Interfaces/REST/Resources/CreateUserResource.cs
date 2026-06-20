using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to create a user.
/// </summary>
[SwaggerSchema(Description = "Request payload to create an organization-scoped user")]
public record CreateUserResource(
    [Required]
    [SwaggerParameter(Description = "User first name")]
    string FirstName,
    [SwaggerParameter(Description = "User last name")]
    string? LastName,
    [Required]
    [EmailAddress]
    [SwaggerParameter(Description = "User email address")]
    string Email,
    [Required]
    [SwaggerParameter(Description = "Role identifier")]
    int RoleId);
