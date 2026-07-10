namespace ColdTrace.Platform.AiAssistance.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while requesting structured AI output.
/// </summary>
public enum AiGenerationError
{
    ProviderDisabled,
    ProviderNotConfigured,
    ProviderUnavailable,
    ProviderTimeout,
    InvalidStructuredOutput,
    UnexpectedError
}
