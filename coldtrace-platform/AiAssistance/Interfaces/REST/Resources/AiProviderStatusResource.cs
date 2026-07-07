namespace ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;

/// <summary>
///     Safe AI provider status resource.
/// </summary>
public record AiProviderStatusResource(
    string Provider,
    string? Model,
    bool Enabled,
    bool Configured,
    bool HasEndpoint,
    bool HasApiKey,
    bool HasChatClient,
    int TimeoutSeconds,
    IReadOnlyCollection<string> StructuredOutputContracts);
