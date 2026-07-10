namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     Uncertainty note owned by an AI resolution plan.
/// </summary>
public class AiResolutionPlanUncertaintyNote
{
    protected AiResolutionPlanUncertaintyNote()
    {
        Value = string.Empty;
    }

    public AiResolutionPlanUncertaintyNote(string value)
    {
        Value = value.Trim();
    }

    public string Value { get; private set; }
}
