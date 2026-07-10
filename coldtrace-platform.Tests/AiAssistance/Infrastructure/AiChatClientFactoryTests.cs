using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.AiAssistance.Infrastructure.Providers;
using Microsoft.Extensions.AI;

namespace ColdTrace.Platform.Tests.AiAssistance.Infrastructure;

public class AiChatClientFactoryTests
{
    [Fact]
    public void Create_WithOpenAiConfiguration_ReturnsChatClient()
    {
        var options = new AiOptions
        {
            Enabled = true,
            Provider = AiProviderNames.OpenAi,
            Model = "gpt-4o-mini",
            ApiKey = "test-api-key"
        };

        using var client = AiChatClientFactory.Create(options);

        Assert.IsAssignableFrom<IChatClient>(client);
    }

    [Fact]
    public void Create_WithOllamaConfiguration_ReturnsChatClient()
    {
        var options = new AiOptions
        {
            Enabled = true,
            Provider = AiProviderNames.Ollama,
            Model = "llama3.2",
            Endpoint = "http://localhost:11434"
        };

        using var client = AiChatClientFactory.Create(options);

        Assert.IsAssignableFrom<IChatClient>(client);
    }

    [Fact]
    public void IsConfigured_WithMalformedOllamaEndpoint_ReturnsFalse()
    {
        var options = new AiOptions
        {
            Enabled = true,
            Provider = AiProviderNames.Ollama,
            Model = "llama3.2",
            Endpoint = "not-a-uri"
        };

        Assert.False(options.IsConfigured);
    }

    [Fact]
    public void Create_WithIncompleteConfiguration_ThrowsInvalidOperationException()
    {
        var options = new AiOptions
        {
            Enabled = true,
            Provider = AiProviderNames.OpenAi,
            Model = "gpt-4o-mini"
        };

        Assert.Throws<InvalidOperationException>(() => AiChatClientFactory.Create(options));
    }
}
