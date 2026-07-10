using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a cold-chain asset.
/// </summary>
[SwaggerSchema(Description = "An asset resource")]
public record AssetResource(
    [SwaggerParameter(Description = "Asset identifier")] int Id,
    [SwaggerParameter(Description = "Organization identifier")] int OrganizationId,
    [SwaggerParameter(Description = "Location identifier")] int LocationId,
    [SwaggerParameter(Description = "Asset unique identifier")] string Uuid,
    [SwaggerParameter(Description = "Business asset type")] string Type,
    [SwaggerParameter(Description = "Asset display name")] string Name,
    [SwaggerParameter(Description = "Asset capacity")] double Capacity,
    [SwaggerParameter(Description = "Optional asset description")] string? Description,
    [SwaggerParameter(Description = "Asset operational status")] string Status);