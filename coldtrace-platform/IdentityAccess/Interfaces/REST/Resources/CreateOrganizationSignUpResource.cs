using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to sign up an organization and its first user.
/// </summary>
/// <param name="LegalName">Organization legal name.</param>
/// <param name="CommercialName">Organization commercial name.</param>
/// <param name="TaxId">Optional organization tax identifier.</param>
/// <param name="ContactEmail">Organization contact email.</param>
/// <param name="FirstName">First user first name.</param>
/// <param name="LastName">First user last name.</param>
/// <param name="Email">First user email address.</param>
[SwaggerSchema(Description = "Request payload to sign up an organization and its first user")]
public record CreateOrganizationSignUpResource(
    [Required]
    [SwaggerParameter(Description = "Organization legal name")]
    string LegalName,
    [Required]
    [SwaggerParameter(Description = "Organization commercial name")]
    string CommercialName,
    [SwaggerParameter(Description = "Optional organization tax identifier")]
    string? TaxId,
    [Required]
    [EmailAddress]
    [SwaggerParameter(Description = "Organization contact email")]
    string ContactEmail,
    [Required]
    [SwaggerParameter(Description = "First user first name")]
    string FirstName,
    [SwaggerParameter(Description = "First user last name")]
    string? LastName,
    [Required]
    [EmailAddress]
    [SwaggerParameter(Description = "First user email address")]
    string Email);
