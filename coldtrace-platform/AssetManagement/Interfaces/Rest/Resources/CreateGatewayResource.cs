using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to create a gateway.
/// </summary>
[SwaggerSchema(Description = "Request payload to create an organization-scoped gateway")]
public record CreateGatewayResource(
    [Required]
    [SwaggerParameter(Description = "Installation location identifier")]
    int LocationId,
    [Required]
    [SwaggerParameter(Description = "Gateway unique identifier")]
    string Uuid,
    [Required]
    [SwaggerParameter(Description = "Gateway name")]
    string Name,
    [Required]
    [SwaggerParameter(Description = "Gateway network")]
    string Network,
    [Required]
    [SwaggerParameter(Description = "Gateway status")]
    string Status);
