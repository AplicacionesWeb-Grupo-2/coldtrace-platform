namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while approving an AI resolution plan.
/// </summary>
public enum ApproveAiResolutionPlanError
{
    OrganizationNotFound,
    IncidentNotFound,
    PlanNotFound,
    PlanAlreadyDecided,
    IncidentAlreadyResolved,
    InvalidIncidentLifecycleTransition,
    UnexpectedError
}
