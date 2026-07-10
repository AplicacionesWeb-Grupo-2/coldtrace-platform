using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to acknowledge an incident.
/// </summary>
[SwaggerSchema(Description = "Request payload for acknowledging an incident")]
public record AcknowledgeIncidentResource(
    [SwaggerParameter(Description = "Actor that acknowledges the incident")]
    string AcknowledgedBy);
