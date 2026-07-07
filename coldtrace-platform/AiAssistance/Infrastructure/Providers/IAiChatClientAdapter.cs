using Microsoft.Extensions.AI;

namespace ColdTrace.Platform.AiAssistance.Infrastructure.Providers;

/// <summary>
///     Adapter that keeps application code decoupled from concrete AI providers.
/// </summary>
public interface IAiChatClientAdapter
{
    bool HasClient { get; }

    IChatClient? GetClient();
}
