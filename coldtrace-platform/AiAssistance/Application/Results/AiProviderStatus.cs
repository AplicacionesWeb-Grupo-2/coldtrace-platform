namespace ColdTrace.Platform.AiAssistance.Application.Results;

/// <summary>
///     Safe AI provider status for diagnostics and API responses.
/// </summary>
public record AiProviderStatus(
    string Provider,
    string? Model,
    bool Enabled,
    bool Configured,
    bool HasEndpoint,
    bool HasApiKey,
    bool HasChatClient,
    int TimeoutSeconds,
    IReadOnlyCollection<string> StructuredOutputContracts);
