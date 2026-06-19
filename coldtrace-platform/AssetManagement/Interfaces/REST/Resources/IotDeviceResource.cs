using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing an IoT device.
/// </summary>
[SwaggerSchema(Description = "An IoT device resource")]
public record IotDeviceResource(
    [SwaggerParameter(Description = "IoT device identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Connected gateway identifier")]
    int GatewayId,
    [SwaggerParameter(Description = "Optional assigned asset identifier")]
    int? AssetId,
    [SwaggerParameter(Description = "Device unique identifier")]
    string Uuid,
    [SwaggerParameter(Description = "Device name")]
    string Name,
    [SwaggerParameter(Description = "Device status")]
    string Status);
