using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

/// <summary>
///     Request resource used to approve a pending AI resolution plan.
/// </summary>
[SwaggerSchema(Description = "Request payload for approving an AI plan and resolving the incident")]
public record ApproveAiResolutionPlanResource(
    [SwaggerParameter(Description = "Actor that approves the plan")]
    string ApprovedBy,
    [SwaggerParameter(Description = "Final operator-approved corrective action")]
    string FinalCorrectiveAction,
    [SwaggerParameter(Description = "Final operator-approved resolution notes")]
    string FinalResolutionNotes);
