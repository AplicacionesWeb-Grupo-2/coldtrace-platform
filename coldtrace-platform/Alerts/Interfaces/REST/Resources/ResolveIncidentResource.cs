using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to resolve an incident.
/// </summary>
[SwaggerSchema(Description = "Request payload for resolving an incident")]
public record ResolveIncidentResource(
    [SwaggerParameter(Description = "Actor that resolves the incident")]
    string ResolvedBy,
    [SwaggerParameter(Description = "Resolution notes")]
    string ResolutionNotes);
