using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to manually register an incident.
/// </summary>
[SwaggerSchema(Description = "Request payload for registering an incident")]
public record CreateIncidentResource(
    [SwaggerParameter(Description = "Optional related asset identifier")]
    int? AssetId,
    [SwaggerParameter(Description = "Optional related IoT device identifier")]
    int? DeviceId,
    [SwaggerParameter(Description = "Optional related sensor reading identifier")]
    int? ReadingId,
    [SwaggerParameter(Description = "Optional asset display name snapshot")]
    string? AssetName,
    [SwaggerParameter(Description = "Optional device display name snapshot")]
    string? DeviceName,
    [SwaggerParameter(Description = "Incident type")]
    string Type,
    [SwaggerParameter(Description = "Incident severity: warning or critical")]
    string Severity,
    [SwaggerParameter(Description = "Detected or reported value")]
    string? Value);
