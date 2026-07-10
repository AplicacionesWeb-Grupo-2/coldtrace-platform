using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to authenticate an existing user.
/// </summary>
[SwaggerSchema(Description = "User sign-in request with email and password")]
public record SignInResource(
    [property: JsonRequired]
    [Required]
    [EmailAddress]
    [SwaggerParameter(Description = "User email address")]
    string Email,
    [property: JsonRequired]
    [Required]
    [SwaggerParameter(Description = "User password")]
    string Password);
