using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a notification read model.
/// </summary>
[SwaggerSchema(Description = "An incident notification resource")]
public record NotificationResource(
    [SwaggerParameter(Description = "Notification identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Incident identifier")]
    int IncidentId,
    [SwaggerParameter(Description = "Notification channel")]
    string Channel,
    [SwaggerParameter(Description = "Notification recipient")]
    string? Recipient,
    [SwaggerParameter(Description = "Notification message")]
    string Message,
    [SwaggerParameter(Description = "Notification status")]
    string Status,
    [SwaggerParameter(Description = "Creation timestamp")]
    DateTimeOffset? CreatedAt,
    [SwaggerParameter(Description = "Delivery timestamp")]
    DateTimeOffset? DeliveredAt,
    [SwaggerParameter(Description = "Failure reason")]
    string? FailureReason);
