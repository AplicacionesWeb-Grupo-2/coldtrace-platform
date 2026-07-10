using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing an organization.
/// </summary>
/// <param name="Id">Organization identifier.</param>
/// <param name="LegalName">Organization legal name.</param>
/// <param name="CommercialName">Organization commercial name.</param>
/// <param name="TaxId">Organization tax identifier.</param>
/// <param name="ContactEmail">Organization contact email.</param>
[SwaggerSchema(Description = "An organization resource")]
public record OrganizationResource(
    [SwaggerParameter(Description = "Organization identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization legal name")]
    string LegalName,
    [SwaggerParameter(Description = "Organization commercial name")]
    string CommercialName,
    [SwaggerParameter(Description = "Organization tax identifier")]
    string? TaxId,
    [SwaggerParameter(Description = "Organization contact email")]
    string ContactEmail);
