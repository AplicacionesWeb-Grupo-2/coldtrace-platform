namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;

/// <summary>
///     Command for opening a corrective technical service request.
/// </summary>
public record CreateTechnicalServiceRequestCommand
{
    private static readonly HashSet<string> ValidPriorities = ["low", "medium", "high"];

    /// <summary>
    ///     Creates a command with validated and normalized technical service request data.
    /// </summary>
    public CreateTechnicalServiceRequestCommand(
        int organizationId,
        int assetId,
        int? incidentId,
        string issueDescription,
        string priority,
        string? requestedBy)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        AssetId = RequirePositive(assetId, nameof(assetId));
        IncidentId = RequirePositiveWhenPresent(incidentId, nameof(incidentId));
        IssueDescription = RequireNonEmpty(issueDescription, nameof(issueDescription));
        Priority = NormalizePriority(priority);
        RequestedBy = requestedBy?.Trim();
    }

    /// <summary>Gets the owning organization identifier.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the serviced asset identifier.</summary>
    public int AssetId { get; init; }

    /// <summary>Gets the optional related incident identifier.</summary>
    public int? IncidentId { get; init; }

    /// <summary>Gets the reported issue description.</summary>
    public string IssueDescription { get; init; }

    /// <summary>Gets the service priority.</summary>
    public string Priority { get; init; }

    /// <summary>Gets the optional requester name or email.</summary>
    public string? RequestedBy { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static int? RequirePositiveWhenPresent(int? value, string name)
    {
        if (value.HasValue && value.Value <= 0)
            throw new ArgumentException($"{name} must be positive when provided.");
        return value;
    }

    private static string RequireNonEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} is required.");
        return value.Trim();
    }

    private static string NormalizePriority(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("priority is required.");
        var normalized = value.Trim().ToLowerInvariant();
        if (!ValidPriorities.Contains(normalized))
            throw new ArgumentException($"priority '{normalized}' is not supported. Use low, medium, or high.");
        return normalized;
    }
}
