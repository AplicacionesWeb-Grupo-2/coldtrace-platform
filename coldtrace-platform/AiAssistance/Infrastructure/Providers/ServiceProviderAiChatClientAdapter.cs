using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.AiAssistance.Infrastructure.Providers;

/// <summary>
///     Resolves the configured Microsoft.Extensions.AI chat client, if one has been registered.
/// </summary>
public class ServiceProviderAiChatClientAdapter(
    IServiceProvider serviceProvider,
    IOptions<AiOptions> options)
    : IAiChatClientAdapter
{
    /// <inheritdoc />
    public bool HasClient => GetClient() is not null;

    /// <inheritdoc />
    public IChatClient? GetClient() =>
        options.Value.IsConfigured ? serviceProvider.GetService<IChatClient>() : null;
}
