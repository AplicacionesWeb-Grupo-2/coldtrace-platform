using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;

/// <summary>
///     Corrective technical service request aggregate.
/// </summary>
public class TechnicalServiceRequest : IAuditableEntity
{
    private static readonly HashSet<string> TerminalStatuses = ["closed", "canceled"];
    private static readonly HashSet<string> ValidStatuses = ["open", "in_progress", "closed", "canceled"];

    protected TechnicalServiceRequest()
    {
        Code = string.Empty;
        IssueDescription = string.Empty;
        Priority = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    ///     Creates a technical service request from a command and asset data.
    /// </summary>
    public TechnicalServiceRequest(CreateTechnicalServiceRequestCommand command, Asset asset)
    {
        OrganizationId = command.OrganizationId;
        Code = "TSR-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        AssetId = command.AssetId;
        AssetLocationId = asset.LocationId;
        AssetName = asset.Name;
        IncidentId = command.IncidentId;
        IssueDescription = command.IssueDescription;
        Priority = command.Priority;
        Status = "open";
        RequestedBy = command.RequestedBy;
        RequestedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Gets the server-generated identifier.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; private set; }

    /// <summary>Gets the auto-generated public code.</summary>
    public string Code { get; private set; }

    /// <summary>Gets the serviced asset identifier.</summary>
    public int AssetId { get; private set; }

    /// <summary>Gets the asset location snapshot at creation time.</summary>
    public int AssetLocationId { get; private set; }

    /// <summary>Gets the asset name snapshot at creation time.</summary>
    public string? AssetName { get; private set; }

    /// <summary>Gets the optional related incident identifier.</summary>
    public int? IncidentId { get; private set; }

    /// <summary>Gets the reported issue description.</summary>
    public string IssueDescription { get; private set; }

    /// <summary>Gets the service priority.</summary>
    public string Priority { get; private set; }

    /// <summary>Gets the lifecycle status.</summary>
    public string Status { get; private set; }

    /// <summary>Gets the optional requester name or email.</summary>
    public string? RequestedBy { get; private set; }

    /// <summary>Gets the request creation timestamp.</summary>
    public DateTimeOffset RequestedAt { get; private set; }

    /// <summary>Gets the closure timestamp.</summary>
    public DateTimeOffset? ClosedAt { get; private set; }

    /// <summary>Gets the closure summary.</summary>
    public string? ClosureSummary { get; private set; }

    /// <summary>Gets the closure evidence.</summary>
    public string? Evidence { get; private set; }

    /// <summary>Gets the actor who closed the request.</summary>
    public string? ClosedBy { get; private set; }

    /// <summary>Gets the owning organization.</summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>Gets the serviced asset.</summary>
    public Asset Asset { get; private set; } = null!;

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Returns true when the request is not in a terminal status.
    /// </summary>
    public bool IsActive() => !TerminalStatuses.Contains(Status);

    /// <summary>
    ///     Returns true when the current status can transition to the requested status.
    /// </summary>
    public bool CanTransitionTo(string requestedStatus)
    {
        if (!ValidStatuses.Contains(requestedStatus)) return false;
        if (Status == requestedStatus) return true;
        if (TerminalStatuses.Contains(Status)) return false;
        return Status switch
        {
            "open" => requestedStatus is "in_progress" or "closed" or "canceled",
            "in_progress" => requestedStatus is "closed" or "canceled",
            _ => false
        };
    }

    /// <summary>
    ///     Updates the lifecycle status and captures closure data when closing.
    /// </summary>
    public void UpdateStatus(UpdateTechnicalServiceRequestStatusCommand command)
    {
        Status = command.Status;
        if (command.Status == "closed")
        {
            ClosedAt = DateTimeOffset.UtcNow;
            ClosureSummary = command.ClosureSummary;
            Evidence = command.Evidence;
            ClosedBy = command.ClosedBy;
        }
    }
}
