using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to create a location.
/// </summary>
[SwaggerSchema(Description = "Request payload to create an organization-scoped location")]
public record CreateLocationResource(
    [Required]
    [SwaggerParameter(Description = "Location name")]
    string Name,
    [Required]
    [SwaggerParameter(Description = "Location type")]
    string Type,
    [SwaggerParameter(Description = "Optional location address")]
    string? Address,
    [SwaggerParameter(Description = "Optional location description")]
    string? Description,
    [Required]
    [SwaggerParameter(Description = "Location status")]
    string Status);
