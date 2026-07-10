namespace ColdTrace.Platform.Alerts.Application.Errors;

/// <summary>
///     Errors that can occur while generating an AI resolution plan.
/// </summary>
public enum GenerateAiResolutionPlanError
{
    OrganizationNotFound,
    IncidentNotFound,
    IncidentCannotReceivePlans,
    IncidentContextUnavailable,
    AiProviderDisabled,
    AiProviderNotConfigured,
    AiProviderUnavailable,
    AiProviderTimeout,
    InvalidStructuredOutput,
    UnexpectedError
}
