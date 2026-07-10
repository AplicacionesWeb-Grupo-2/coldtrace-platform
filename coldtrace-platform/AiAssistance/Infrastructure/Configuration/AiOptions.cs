namespace ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;

/// <summary>
///     Environment-driven AI assistance provider options.
/// </summary>
public sealed class AiOptions
{
    public const string SectionName = "AiAssistance";

    public bool Enabled { get; set; }

    public string Provider { get; set; } = AiProviderNames.Disabled;

    public string? Model { get; set; }

    public string? Endpoint { get; set; }

    public string? ApiKey { get; set; }

    public string? RequestTimeout { get; set; }

    public int TimeoutSeconds { get; set; } = 30;

    public bool HasProvider => IsSupportedProvider(Provider) &&
                               !string.Equals(Provider, AiProviderNames.Disabled,
                                   StringComparison.OrdinalIgnoreCase);

    public bool HasModel => HasConfiguredValue(Model);

    public bool HasEndpoint => HasConfiguredValue(Endpoint);

    public bool HasValidEndpoint => HasEndpoint &&
                                    Uri.TryCreate(Endpoint, UriKind.Absolute, out var endpoint) &&
                                    (endpoint.Scheme == Uri.UriSchemeHttp || endpoint.Scheme == Uri.UriSchemeHttps);

    public bool HasApiKey => HasConfiguredValue(ApiKey);

    public bool UsesLocalProvider => string.Equals(Provider, AiProviderNames.Ollama,
        StringComparison.OrdinalIgnoreCase);

    public bool RequiresEndpoint => UsesLocalProvider;

    public bool RequiresApiKey => HasProvider && !UsesLocalProvider;

    public bool IsConfigured => Enabled &&
                                HasProvider &&
                                HasModel &&
                                (!RequiresEndpoint || HasValidEndpoint) &&
                                (!RequiresApiKey || HasApiKey);

    public void ExpandEnvironmentVariables()
    {
        Provider = NormalizeProvider(ExpandValue(Provider));
        Model = NormalizeOptionalValue(ExpandValue(Model));
        Endpoint = NormalizeOptionalValue(ExpandValue(Endpoint));
        ApiKey = NormalizeOptionalValue(ExpandValue(ApiKey));
        RequestTimeout = NormalizeOptionalValue(ExpandValue(RequestTimeout));
        TimeoutSeconds = ResolveTimeoutSeconds(RequestTimeout, TimeoutSeconds);
    }

    private static string NormalizeProvider(string? value) =>
        HasConfiguredValue(value) ? value!.Trim().ToLowerInvariant() : AiProviderNames.Disabled;

    private static string? NormalizeOptionalValue(string? value) =>
        HasConfiguredValue(value) ? value!.Trim() : null;

    private static string? ExpandValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? value : Environment.ExpandEnvironmentVariables(value);

    private static bool HasConfiguredValue(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        !value.Contains('%', StringComparison.Ordinal) &&
        !value.StartsWith("$", StringComparison.Ordinal);

    private static bool IsSupportedProvider(string? provider) =>
        string.Equals(provider, AiProviderNames.OpenAi, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(provider, AiProviderNames.Ollama, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(provider, AiProviderNames.Disabled, StringComparison.OrdinalIgnoreCase);

    private static int ResolveTimeoutSeconds(string? requestTimeout, int fallbackSeconds)
    {
        var fallback = Math.Clamp(fallbackSeconds, 1, 120);
        if (!HasConfiguredValue(requestTimeout))
            return fallback;

        var normalized = requestTimeout!.Trim().ToLowerInvariant();
        if (normalized.EndsWith("ms", StringComparison.Ordinal) &&
            int.TryParse(normalized[..^2], out var milliseconds))
            return Math.Clamp((int)Math.Ceiling(milliseconds / 1000d), 1, 120);

        if (normalized.EndsWith("s", StringComparison.Ordinal) &&
            int.TryParse(normalized[..^1], out var seconds))
            return Math.Clamp(seconds, 1, 120);

        if (normalized.EndsWith("m", StringComparison.Ordinal) &&
            int.TryParse(normalized[..^1], out var minutes))
            return Math.Clamp(minutes * 60, 1, 120);

        return int.TryParse(normalized, out var numericSeconds)
            ? Math.Clamp(numericSeconds, 1, 120)
            : fallback;
    }
}
