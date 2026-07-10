using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest.Transform;

/// <summary>
///     Assembles AI provider status resources.
/// </summary>
public static class AiProviderStatusResourceFromResultAssembler
{
    public static AiProviderStatusResource ToResourceFromResult(AiProviderStatus status) =>
        new(
            status.Provider,
            status.Model,
            status.Enabled,
            status.Configured,
            status.HasEndpoint,
            status.HasApiKey,
            status.HasChatClient,
            status.TimeoutSeconds,
            status.StructuredOutputContracts);
}
