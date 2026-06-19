using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     Notification read model derived from incident lifecycle events.
/// </summary>
public class Notification : IAuditableEntity
{
    public const string ChannelApp = "app";
    public const string StatusSent = "sent";

    protected Notification()
    {
        Channel = string.Empty;
        Message = string.Empty;
        Status = string.Empty;
    }

    private Notification(int organizationId, int incidentId, string? recipient, string message)
    {
        OrganizationId = organizationId;
        IncidentId = incidentId;
        Channel = ChannelApp;
        Recipient = recipient;
        Message = message;
        Status = StatusSent;
        DeliveredAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public int OrganizationId { get; private set; }

    public int IncidentId { get; private set; }

    public string Channel { get; private set; }

    public string? Recipient { get; private set; }

    public string Message { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset? DeliveredAt { get; private set; }

    public string? FailureReason { get; private set; }

    public Organization Organization { get; private set; } = null!;

    public Incident Incident { get; private set; } = null!;

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public static Notification IncidentOpened(Incident incident) =>
        new(
            incident.OrganizationId,
            incident.Id,
            null,
            $"Incident {incident.Id} opened with {incident.Severity} severity");

    public static Notification IncidentAcknowledged(Incident incident) =>
        new(
            incident.OrganizationId,
            incident.Id,
            incident.AcknowledgedBy,
            $"Incident {incident.Id} acknowledged");

    public static Notification IncidentResolved(Incident incident) =>
        new(
            incident.OrganizationId,
            incident.Id,
            incident.ResolvedBy,
            $"Incident {incident.Id} resolved");
}
