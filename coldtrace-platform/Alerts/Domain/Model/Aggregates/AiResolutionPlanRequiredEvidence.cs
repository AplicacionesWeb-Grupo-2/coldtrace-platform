namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     Evidence item owned by an AI resolution plan.
/// </summary>
public class AiResolutionPlanRequiredEvidence
{
    protected AiResolutionPlanRequiredEvidence()
    {
        Value = string.Empty;
    }

    public AiResolutionPlanRequiredEvidence(string value)
    {
        Value = value.Trim();
    }

    public string Value { get; private set; }
}
