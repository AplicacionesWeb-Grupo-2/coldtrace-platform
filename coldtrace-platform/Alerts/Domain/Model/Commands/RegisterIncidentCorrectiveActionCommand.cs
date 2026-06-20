namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for registering corrective action on an active incident.
/// </summary>
public record RegisterIncidentCorrectiveActionCommand
{
    public RegisterIncidentCorrectiveActionCommand(
        int organizationId,
        int incidentId,
        string correctiveAction,
        string registeredBy)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        CorrectiveAction = RequireNonBlank(correctiveAction);
        RegisteredBy = RequireNonBlank(registeredBy);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public string CorrectiveAction { get; init; }

    public string RegisteredBy { get; init; }

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
