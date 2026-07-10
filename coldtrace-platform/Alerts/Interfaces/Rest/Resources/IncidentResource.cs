using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing an incident.
/// </summary>
[SwaggerSchema(Description = "An incident resource")]
public record IncidentResource(
    [SwaggerParameter(Description = "Incident identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Optional asset identifier")]
    int? AssetId,
    [SwaggerParameter(Description = "Optional IoT device identifier")]
    int? DeviceId,
    [SwaggerParameter(Description = "Optional sensor reading identifier")]
    int? ReadingId,
    [SwaggerParameter(Description = "Optional asset display name snapshot")]
    string? AssetName,
    [SwaggerParameter(Description = "Optional device display name snapshot")]
    string? DeviceName,
    [SwaggerParameter(Description = "Incident type")]
    string Type,
    [SwaggerParameter(Description = "Incident severity")]
    string Severity,
    [SwaggerParameter(Description = "Incident lifecycle status")]
    string Status,
    [SwaggerParameter(Description = "Detected or reported value")]
    string? Value,
    [SwaggerParameter(Description = "Detection timestamp")]
    DateTimeOffset DetectedAt,
    [SwaggerParameter(Description = "Acknowledgement timestamp")]
    DateTimeOffset? AcknowledgedAt,
    [SwaggerParameter(Description = "Actor that acknowledged the incident")]
    string? AcknowledgedBy,
    [SwaggerParameter(Description = "Escalation timestamp")]
    DateTimeOffset? EscalatedAt,
    [SwaggerParameter(Description = "Actor that escalated the incident")]
    string? EscalatedBy,
    [SwaggerParameter(Description = "Reason that justified escalation")]
    string? EscalationReason,
    [SwaggerParameter(Description = "Corrective action registration timestamp")]
    DateTimeOffset? CorrectiveActionRegisteredAt,
    [SwaggerParameter(Description = "Actor that registered the corrective action")]
    string? CorrectiveActionRegisteredBy,
    [SwaggerParameter(Description = "Corrective action details")]
    string? CorrectiveAction,
    [SwaggerParameter(Description = "Resolution timestamp")]
    DateTimeOffset? ResolvedAt,
    [SwaggerParameter(Description = "Actor that resolved the incident")]
    string? ResolvedBy,
    [SwaggerParameter(Description = "Resolution notes")]
    string? ResolutionNotes,
    [SwaggerParameter(Description = "Last notification status")]
    string? LastNotificationStatus,
    [SwaggerParameter(Description = "Last notification timestamp")]
    DateTimeOffset? LastNotificationAt,
    [SwaggerParameter(Description = "Notification count")]
    int NotificationCount);
