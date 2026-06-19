namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for acknowledging an open incident.
/// </summary>
public record AcknowledgeIncidentCommand
{
    public AcknowledgeIncidentCommand(int organizationId, int incidentId, string acknowledgedBy)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        AcknowledgedBy = RequireNonBlank(acknowledgedBy);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public string AcknowledgedBy { get; init; }

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
