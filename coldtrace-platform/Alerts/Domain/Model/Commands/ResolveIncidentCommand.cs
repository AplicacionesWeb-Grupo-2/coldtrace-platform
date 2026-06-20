namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for resolving an active incident.
/// </summary>
public record ResolveIncidentCommand
{
    public ResolveIncidentCommand(int organizationId, int incidentId, string resolvedBy, string resolutionNotes)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        ResolvedBy = RequireNonBlank(resolvedBy);
        ResolutionNotes = RequireNonBlank(resolutionNotes);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public string ResolvedBy { get; init; }

    public string ResolutionNotes { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
