using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to assign a role to an organization user.
/// </summary>
[SwaggerSchema(Description = "Request payload to assign or replace an organization user's role")]
public record AssignUserRoleResource(
    [Required]
    [Range(1, int.MaxValue)]
    [SwaggerParameter(Description = "Target role identifier")]
    int RoleId);
