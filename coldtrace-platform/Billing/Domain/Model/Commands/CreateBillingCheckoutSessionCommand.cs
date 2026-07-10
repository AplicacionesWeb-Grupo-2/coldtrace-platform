namespace ColdTrace.Platform.Billing.Domain.Model.Commands;

/// <summary>
///     Command for creating a provider-hosted checkout session for a paid plan.
/// </summary>
public record CreateBillingCheckoutSessionCommand
{
    public CreateBillingCheckoutSessionCommand(int organizationId, string targetPlanCode)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(targetPlanCode))
            throw new ArgumentException("Target plan code is required.", nameof(targetPlanCode));

        OrganizationId = organizationId;
        TargetPlanCode = targetPlanCode.Trim().ToLowerInvariant();
    }

    public int OrganizationId { get; init; }

    public string TargetPlanCode { get; init; }
}
