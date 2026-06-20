using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to register corrective action on an incident.
/// </summary>
[SwaggerSchema(Description = "Request payload for registering corrective action on an active incident")]
public record RegisterIncidentCorrectiveActionResource(
    [Required]
    [SwaggerParameter(Description = "Corrective action applied to the incident")]
    string CorrectiveAction,
    [Required]
    [SwaggerParameter(Description = "Actor that registers the corrective action")]
    string RegisteredBy);
