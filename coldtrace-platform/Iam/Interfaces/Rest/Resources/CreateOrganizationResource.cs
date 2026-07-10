using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to create an organization.
/// </summary>
/// <param name="LegalName">Organization legal name.</param>
/// <param name="CommercialName">Organization commercial name.</param>
/// <param name="TaxId">Optional organization tax identifier.</param>
/// <param name="ContactEmail">Organization contact email.</param>
[SwaggerSchema(Description = "Request payload to create an organization")]
public record CreateOrganizationResource(
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
    string ContactEmail);
