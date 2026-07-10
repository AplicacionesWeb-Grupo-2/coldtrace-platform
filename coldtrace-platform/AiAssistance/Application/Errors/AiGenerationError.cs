namespace ColdTrace.Platform.AiAssistance.Application.Errors;

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
