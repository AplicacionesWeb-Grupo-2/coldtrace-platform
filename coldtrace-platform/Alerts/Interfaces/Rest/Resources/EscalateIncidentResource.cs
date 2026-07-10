using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to escalate an incident.
/// </summary>
[SwaggerSchema(Description = "Request payload for escalating an active incident")]
public record EscalateIncidentResource(
    [Required]
    [SwaggerParameter(Description = "Actor that escalates the incident")]
    string EscalatedBy,
    [Required]
    [SwaggerParameter(Description = "Reason for escalation")]
    string EscalationReason);
