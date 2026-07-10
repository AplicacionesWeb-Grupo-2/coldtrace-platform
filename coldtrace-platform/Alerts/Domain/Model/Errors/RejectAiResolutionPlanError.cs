namespace ColdTrace.Platform.Alerts.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while rejecting an AI resolution plan.
/// </summary>
public enum RejectAiResolutionPlanError
{
    OrganizationNotFound,
    IncidentNotFound,
    PlanNotFound,
    PlanAlreadyDecided,
    UnexpectedError
}
