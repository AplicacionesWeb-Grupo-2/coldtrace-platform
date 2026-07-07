namespace ColdTrace.Platform.Alerts.Domain.Model.Commands;

/// <summary>
///     Command for rejecting a pending AI resolution plan.
/// </summary>
public record RejectAiResolutionPlanCommand
{
    public RejectAiResolutionPlanCommand(
        int organizationId,
        int incidentId,
        int planId,
        string rejectedBy,
        string rejectionReason)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        IncidentId = RequirePositive(incidentId, nameof(incidentId));
        PlanId = RequirePositive(planId, nameof(planId));
        RejectedBy = RequireNonBlank(rejectedBy);
        RejectionReason = RequireNonBlank(rejectionReason);
    }

    public int OrganizationId { get; init; }

    public int IncidentId { get; init; }

    public int PlanId { get; init; }

    public string RejectedBy { get; init; }

    public string RejectionReason { get; init; }

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
