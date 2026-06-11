using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing an operational location.
/// </summary>
/// <param name="Id">Location identifier.</param>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="Name">Location name.</param>
/// <param name="Type">Location type.</param>
/// <param name="Address">Optional location address.</param>
/// <param name="Description">Optional location description.</param>
/// <param name="Status">Location status.</param>
[SwaggerSchema(Description = "A location resource")]
public record LocationResource(
    [SwaggerParameter(Description = "Location identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Location name")]
    string Name,
    [SwaggerParameter(Description = "Location type")]
    string Type,
    [SwaggerParameter(Description = "Optional location address")]
    string? Address,
    [SwaggerParameter(Description = "Optional location description")]
    string? Description,
    [SwaggerParameter(Description = "Location status")]
    string Status);
