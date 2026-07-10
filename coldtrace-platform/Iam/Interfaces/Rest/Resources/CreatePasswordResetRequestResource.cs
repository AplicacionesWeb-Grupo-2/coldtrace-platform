using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Request payload used to request a password reset.
/// </summary>
[SwaggerSchema(Description = "Password reset request payload")]
public record CreatePasswordResetRequestResource(
    [Required]
    [EmailAddress]
    [SwaggerParameter(Description = "User email address")]
    string Email);
