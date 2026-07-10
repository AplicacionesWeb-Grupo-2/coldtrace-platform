namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for approving a pending AI resolution plan.
/// </summary>
public record ApproveAiResolutionPlanCommand
{
    public ApproveAiResolutionPlanCommand(
        int organizationId,
        int incidentId,
        int planId,
        string approvedBy,
        string finalCorrectiveAction,
        string finalResolutionNotes)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        PlanId = RequirePositive(planId, nameof(planId));
        ApprovedBy = RequireNonBlank(approvedBy);
        FinalCorrectiveAction = RequireNonBlank(finalCorrectiveAction);
        FinalResolutionNotes = RequireNonBlank(finalResolutionNotes);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public int PlanId { get; init; }

    public string ApprovedBy { get; init; }

    public string FinalCorrectiveAction { get; init; }

    public string FinalResolutionNotes { get; init; }

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
