using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ColdTrace.Platform.AiAssistance.Infrastructure.Providers;

/// <summary>
///     Creates the configured provider client behind the Microsoft.Extensions.AI abstraction.
/// </summary>
public static class AiChatClientFactory
{
    /// <summary>
    ///     Creates a chat client for the configured provider.
    /// </summary>
    /// <param name="options">Validated AI provider configuration.</param>
    /// <returns>The provider-independent chat client.</returns>
    /// <exception cref="InvalidOperationException">The configuration is incomplete or unsupported.</exception>
    public static IChatClient Create(AiOptions options)
    {
        if (!options.IsConfigured)
            throw new InvalidOperationException("AI assistance provider configuration is incomplete.");

        return options.Provider switch
        {
            AiProviderNames.OpenAi => CreateOpenAiClient(options),
            AiProviderNames.Ollama => CreateOllamaClient(options),
            _ => throw new InvalidOperationException($"Unsupported AI assistance provider: {options.Provider}.")
        };
    }

    private static IChatClient CreateOpenAiClient(AiOptions options)
    {
        var chatClient = new ChatClient(
            options.Model!,
            new ApiKeyCredential(options.ApiKey!),
            new OpenAIClientOptions());

        return chatClient.AsIChatClient();
    }

    private static IChatClient CreateOllamaClient(AiOptions options) =>
        new OllamaApiClient(new Uri(options.Endpoint!, UriKind.Absolute), options.Model!);
}
