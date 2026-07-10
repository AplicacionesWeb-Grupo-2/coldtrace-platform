using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to reject a pending AI resolution plan.
/// </summary>
[SwaggerSchema(Description = "Request payload for rejecting an AI plan without changing incident state")]
public record RejectAiResolutionPlanResource(
    [SwaggerParameter(Description = "Actor that rejects the plan")]
    string RejectedBy,
    [SwaggerParameter(Description = "Reason recorded for audit history")]
    string RejectionReason);
