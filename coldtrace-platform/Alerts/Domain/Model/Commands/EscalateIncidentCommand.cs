namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for escalating an active incident.
/// </summary>
public record EscalateIncidentCommand
{
    public EscalateIncidentCommand(
        int organizationId,
        int incidentId,
        string escalatedBy,
        string escalationReason)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        EscalatedBy = RequireNonBlank(escalatedBy);
        EscalationReason = RequireNonBlank(escalationReason);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public string EscalatedBy { get; init; }

    public string EscalationReason { get; init; }

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
