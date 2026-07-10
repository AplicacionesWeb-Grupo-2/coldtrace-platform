using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a notification resource from a read model entity.
/// </summary>
public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification notification) =>
        new(
            notification.Id,
            notification.OrganizationId,
            notification.IncidentId,
            notification.Channel,
            notification.Recipient,
            notification.Message,
            notification.Status,
            notification.CreatedAt,
            notification.DeliveredAt,
            notification.FailureReason);
}
