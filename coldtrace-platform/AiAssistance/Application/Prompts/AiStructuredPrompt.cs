namespace ColdTrace.Platform.AiAssistance.Application.Prompts;

/// <summary>
///     Provider-neutral prompt input used by AI application services.
/// </summary>
public record AiStructuredPrompt(
    string SystemInstruction,
    string UserRequest,
    IReadOnlyDictionary<string, string>? Context = null);
