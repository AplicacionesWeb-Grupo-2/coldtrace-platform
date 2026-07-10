namespace ColdTrace.Platform.AiAssistance.Domain.Model.Errors;

/// <summary>
///     Controlled errors for dashboard AI interpretation generation.
/// </summary>
public enum GenerateDashboardAiInterpretationError
{
    OrganizationNotFound,
    DashboardContextUnavailable,
    AiProviderDisabled,
    AiProviderNotConfigured,
    AiProviderUnavailable,
    AiProviderTimeout,
    InvalidStructuredOutput,
    UnexpectedError
}
