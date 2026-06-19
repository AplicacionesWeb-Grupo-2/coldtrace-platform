using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     Incident aggregate for monitoring alerts owned by the backend.
/// </summary>
public class Incident : IAuditableEntity
{
    public const string SeverityWarning = "warning";
    public const string SeverityCritical = "critical";
    public const string StatusOpen = "open";
    public const string StatusAcknowledged = "acknowledged";
    public const string StatusResolved = "resolved";

    protected Incident()
    {
        Type = string.Empty;
        Severity = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates an incident from a validated command.
    /// </summary>
    public Incident(CreateIncidentCommand command)
    {
        OrganizationId = command.OrganizationId;
        AssetId = command.AssetId;
        DeviceId = command.DeviceId;
        ReadingId = command.ReadingId;
        AssetName = command.AssetName;
        DeviceName = command.DeviceName;
        Type = command.Type;
        Severity = command.Severity;
        Status = StatusOpen;
        Value = command.Value;
        DetectedAt = DateTimeOffset.UtcNow;
        NotificationCount = 0;
    }

    public int Id { get; private set; }

    public int OrganizationId { get; private set; }

    public int? AssetId { get; private set; }

    public int? DeviceId { get; private set; }

    public int? ReadingId { get; private set; }

    public string? AssetName { get; private set; }

    public string? DeviceName { get; private set; }

    public string Type { get; private set; }

    public string Severity { get; private set; }

    public string Status { get; private set; }

    public string? Value { get; private set; }

    public DateTimeOffset DetectedAt { get; private set; }

    public DateTimeOffset? AcknowledgedAt { get; private set; }

    public string? AcknowledgedBy { get; private set; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public string? ResolvedBy { get; private set; }

    public string? ResolutionNotes { get; private set; }

    public string? LastNotificationStatus { get; private set; }

    public DateTimeOffset? LastNotificationAt { get; private set; }

    public int NotificationCount { get; private set; }

    public Organization Organization { get; private set; } = null!;

    public Asset? Asset { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public bool IsOpen() => Status == StatusOpen;

    public bool IsAcknowledged() => Status == StatusAcknowledged;

    public bool IsResolved() => Status == StatusResolved;

    /// <summary>
    ///     Acknowledges an open incident.
    /// </summary>
    public void Acknowledge(AcknowledgeIncidentCommand command)
    {
        Status = StatusAcknowledged;
        AcknowledgedAt = DateTimeOffset.UtcNow;
        AcknowledgedBy = command.AcknowledgedBy;
    }

    /// <summary>
    ///     Resolves an open or acknowledged incident.
    /// </summary>
    public void Resolve(ResolveIncidentCommand command)
    {
        Status = StatusResolved;
        ResolvedAt = DateTimeOffset.UtcNow;
        ResolvedBy = command.ResolvedBy;
        ResolutionNotes = command.ResolutionNotes;
    }

    /// <summary>
    ///     Records the last emitted notification state.
    /// </summary>
    public void RecordNotification(string status)
    {
        LastNotificationStatus = status;
        LastNotificationAt = DateTimeOffset.UtcNow;
        NotificationCount += 1;
    }
}
