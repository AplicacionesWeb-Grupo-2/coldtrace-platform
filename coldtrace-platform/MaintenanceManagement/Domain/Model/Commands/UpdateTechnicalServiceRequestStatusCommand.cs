namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;

/// <summary>
///     Command for updating the lifecycle status of a technical service request.
///     When transitioning to "closed", closure fields are mandatory.
/// </summary>
public record UpdateTechnicalServiceRequestStatusCommand
{
    private static readonly HashSet<string> ValidStatuses = ["open", "in_progress", "closed", "canceled"];

    /// <summary>
    ///     Creates a command with validated status data.
    /// </summary>
    public UpdateTechnicalServiceRequestStatusCommand(
        int organizationId,
        int technicalServiceRequestId,
        string status,
        string? closureSummary,
        string? evidence,
        string? closedBy)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        TechnicalServiceRequestId = RequirePositive(technicalServiceRequestId, nameof(technicalServiceRequestId));
        Status = NormalizeStatus(status);

        if (Status == "closed")
        {
            ClosureSummary = RequireNonEmptyForClosure(closureSummary, nameof(closureSummary));
            Evidence = RequireNonEmptyForClosure(evidence, nameof(evidence));
            ClosedBy = RequireNonEmptyForClosure(closedBy, nameof(closedBy));
        }
        else
        {
            ClosureSummary = closureSummary?.Trim();
            Evidence = evidence?.Trim();
            ClosedBy = closedBy?.Trim();
        }
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the technical service request identifier.</summary>
    public int TechnicalServiceRequestId { get; init; }

    /// <summary>Gets the target lifecycle status.</summary>
    public string Status { get; init; }

    /// <summary>Gets the closure summary (required when closing).</summary>
    public string? ClosureSummary { get; init; }

    /// <summary>Gets the closure evidence (required when closing).</summary>
    public string? Evidence { get; init; }

    /// <summary>Gets the actor who closed the request (required when closing).</summary>
    public string? ClosedBy { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string NormalizeStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("status is required.");
        var normalized = value.Trim().ToLowerInvariant();
        if (!ValidStatuses.Contains(normalized))
            throw new ArgumentException($"status '{normalized}' is not supported.");
        return normalized;
    }

    private static string RequireNonEmptyForClosure(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} is required when closing a technical service request.");
        return value.Trim();
    }
}
