using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing one AI resolution plan step.
/// </summary>
[SwaggerSchema(Description = "An AI resolution plan step resource")]
public record AiResolutionPlanStepResource(
    [SwaggerParameter(Description = "Step sequence number")]
    int Sequence,
    [SwaggerParameter(Description = "Recommended action")]
    string Action,
    [SwaggerParameter(Description = "Reasoning for the action")]
    string Rationale,
    [SwaggerParameter(Description = "Expected outcome")]
    string ExpectedOutcome);
