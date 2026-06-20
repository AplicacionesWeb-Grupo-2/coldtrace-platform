using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to update an asset.
/// </summary>
[SwaggerSchema(Description = "Request payload to update an organization-scoped asset")]
public record UpdateAssetResource(
    [Required]
    [SwaggerParameter(Description = "Placement location identifier")]
    int LocationId,
    [Required]
    [SwaggerParameter(Description = "Asset unique identifier inside the organization")]
    string Uuid,
    [Required]
    [SwaggerParameter(Description = "Business asset type")]
    string Type,
    [Required]
    [SwaggerParameter(Description = "Asset display name")]
    string Name,
    [SwaggerParameter(Description = "Asset capacity")]
    double Capacity,
    [SwaggerParameter(Description = "Optional asset description")]
    string? Description,
    [Required]
    [SwaggerParameter(Description = "Asset operational status")]
    string Status);