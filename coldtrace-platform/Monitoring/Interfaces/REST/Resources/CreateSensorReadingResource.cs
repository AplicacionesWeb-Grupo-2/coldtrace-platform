using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to create a sensor reading.
/// </summary>
[SwaggerSchema(Description = "Request payload to create a sensor reading for an IoT device")]
public record CreateSensorReadingResource(
    [Required]
    [SwaggerParameter(Description = "Reading metric name")]
    string Metric,
    [SwaggerParameter(Description = "Reading value")]
    decimal Value,
    [Required]
    [SwaggerParameter(Description = "Reading unit")]
    string Unit,
    [SwaggerParameter(Description = "Optional timestamp when the reading was captured")]
    DateTimeOffset? RecordedAt);
