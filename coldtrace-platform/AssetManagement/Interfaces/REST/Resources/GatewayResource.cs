using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing an edge gateway.
/// </summary>
/// <param name="Id">Gateway identifier.</param>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="LocationId">Location identifier.</param>
/// <param name="Uuid">Gateway unique identifier.</param>
/// <param name="Name">Gateway name.</param>
/// <param name="Network">Gateway network.</param>
/// <param name="Status">Gateway status.</param>
[SwaggerSchema(Description = "A gateway resource")]
public record GatewayResource(
    [SwaggerParameter(Description = "Gateway identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Location identifier")]
    int LocationId,
    [SwaggerParameter(Description = "Gateway unique identifier")]
    string Uuid,
    [SwaggerParameter(Description = "Gateway name")]
    string Name,
    [SwaggerParameter(Description = "Gateway network")]
    string Network,
    [SwaggerParameter(Description = "Gateway status")]
    string Status);
