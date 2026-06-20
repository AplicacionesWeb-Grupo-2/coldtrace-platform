using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an incident resource from an aggregate.
/// </summary>
public static class IncidentResourceFromEntityAssembler
{
    public static IncidentResource ToResourceFromEntity(Incident incident) =>
        new(
            incident.Id,
            incident.OrganizationId,
            incident.AssetId,
            incident.DeviceId,
            incident.ReadingId,
            incident.AssetName,
            incident.DeviceName,
            incident.Type,
            incident.Severity,
            incident.Status,
            incident.Value,
            incident.DetectedAt,
            incident.AcknowledgedAt,
            incident.AcknowledgedBy,
            incident.EscalatedAt,
            incident.EscalatedBy,
            incident.EscalationReason,
            incident.CorrectiveActionRegisteredAt,
            incident.CorrectiveActionRegisteredBy,
            incident.CorrectiveAction,
            incident.ResolvedAt,
            incident.ResolvedBy,
            incident.ResolutionNotes,
            incident.LastNotificationStatus,
            incident.LastNotificationAt,
            incident.NotificationCount);
}
