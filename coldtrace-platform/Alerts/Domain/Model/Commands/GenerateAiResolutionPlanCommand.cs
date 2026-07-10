namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for generating an AI-backed resolution plan for an incident.
/// </summary>
public record GenerateAiResolutionPlanCommand
{
    public GenerateAiResolutionPlanCommand(int organizationId, int incidentId)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
