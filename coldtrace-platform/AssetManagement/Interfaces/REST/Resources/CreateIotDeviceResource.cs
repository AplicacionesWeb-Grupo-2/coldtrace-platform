using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to create an IoT device.
/// </summary>
[SwaggerSchema(Description = "Request payload to create an organization-scoped IoT device")]
public record CreateIotDeviceResource(
    [Required]
    [SwaggerParameter(Description = "Connected gateway identifier")]
    int GatewayId,
    [SwaggerParameter(Description = "Optional assigned asset identifier")]
    int? AssetId,
    [Required]
    [SwaggerParameter(Description = "Device unique identifier")]
    string Uuid,
    [Required]
    [SwaggerParameter(Description = "Device name")]
    string Name,
    [Required]
    [SwaggerParameter(Description = "Device status")]
    string Status);
