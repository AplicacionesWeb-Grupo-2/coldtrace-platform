using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to generate demo sensor readings.
/// </summary>
[SwaggerSchema(Description = "Request payload for backend-owned demo telemetry generation")]
public record GenerateDemoSensorReadingsResource(
    [SwaggerParameter(Description = "Optional asset filter")]
    int? AssetId,
    [Range(1, 50)]
    [SwaggerParameter(Description = "Number of readings to generate")]
    int? Count);
