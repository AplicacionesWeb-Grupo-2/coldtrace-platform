namespace ColdTrace.Platform.Alerts.Domain.Model.Aggregates;

/// <summary>
///     Recommended step owned by an AI resolution plan.
/// </summary>
public class AiResolutionPlanStep
{
    protected AiResolutionPlanStep()
    {
        Action = string.Empty;
        Rationale = string.Empty;
        ExpectedOutcome = string.Empty;
    }

    public AiResolutionPlanStep(int sequence, string action, string rationale, string expectedOutcome)
    {
        Sequence = sequence;
        Action = action.Trim();
        Rationale = rationale.Trim();
        ExpectedOutcome = expectedOutcome.Trim();
    }

    public int Sequence { get; private set; }

    public string Action { get; private set; }

    public string Rationale { get; private set; }

    public string ExpectedOutcome { get; private set; }
}
