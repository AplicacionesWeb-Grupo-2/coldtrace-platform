using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Internal.OutboundServices.Social;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Iam.Infrastructure.OAuth.Configuration;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ColdTrace.Platform.Iam.Infrastructure.OAuth.Services;

/// <summary>
///     Validates Google and Apple OIDC identities against provider JWKS documents.
/// </summary>
public sealed class OidcExternalIdentityProviderService : IExternalIdentityProviderService
{
    public const string HttpClientName = "SocialAuthentication";

    private const string AppleClientSecretAudience = "https://appleid.apple.com";
    private static readonly TimeSpan JwksCacheTtl = TimeSpan.FromMinutes(10);

    private readonly ConcurrentDictionary<SocialProvider, CachedJwks> _jwksCache = new();
    private readonly ConcurrentDictionary<SocialProvider, SemaphoreSlim> _jwksRefreshLocks = new();
    private readonly HttpClient _httpClient;
    private readonly JsonWebTokenHandler _tokenHandler = new();
    private readonly SocialAuthenticationOptions _options;
    private readonly ILogger<OidcExternalIdentityProviderService> _logger;

    public OidcExternalIdentityProviderService(
        IHttpClientFactory httpClientFactory,
        IOptions<SocialAuthenticationOptions> options,
        ILogger<OidcExternalIdentityProviderService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClient = httpClientFactory.CreateClient(HttpClientName);
        _options = options.Value;
        _options.ExpandEnvironmentVariables();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<ProviderIdentity, SocialAuthenticationError>> ValidateAsync(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var configuration = GetProviderConfiguration(command.Provider);
            var idToken = await ResolveIdTokenAsync(command, configuration, cancellationToken);
            var identity = await ValidateIdTokenAsync(command, configuration, idToken, cancellationToken);
            return new Result<ProviderIdentity, SocialAuthenticationError>.Success(identity);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ProviderConfigurationException)
        {
            _logger.LogWarning(
                "Social identity provider configuration is incomplete for {Provider}.",
                command.Provider);
            return new Result<ProviderIdentity, SocialAuthenticationError>.Failure(
                SocialAuthenticationError.ProviderConfigurationMissing());
        }
        catch (Exception)
        {
            _logger.LogWarning(
                "Social identity provider validation failed for {Provider}.",
                command.Provider);
            return new Result<ProviderIdentity, SocialAuthenticationError>.Failure(
                SocialAuthenticationError.ProviderValidationFailed());
        }
    }

    private ProviderConfiguration GetProviderConfiguration(SocialProvider provider)
    {
        ProviderConfiguration configuration = provider switch
        {
            SocialProvider.Google => new ProviderConfiguration(
                provider,
                _options.Google.ClientId,
                _options.Google.ClientSecret,
                _options.Google.RedirectUri,
                _options.Google.TokenUri,
                _options.Google.JwksUri,
                _options.Google.Issuer,
                null,
                null,
                null),
            SocialProvider.Apple => new ProviderConfiguration(
                provider,
                _options.Apple.ClientId,
                null,
                _options.Apple.RedirectUri,
                _options.Apple.TokenUri,
                _options.Apple.JwksUri,
                _options.Apple.Issuer,
                _options.Apple.TeamId,
                _options.Apple.KeyId,
                _options.Apple.PrivateKey),
            _ => throw new ProviderConfigurationException()
        };

        if (!HasText(configuration.ClientId) ||
            !HasText(configuration.JwksUri) ||
            !HasText(configuration.Issuer))
            throw new ProviderConfigurationException();

        return configuration with { ClientId = configuration.ClientId!.Trim() };
    }

    private async Task<string> ResolveIdTokenAsync(
        SocialSignInCommand command,
        ProviderConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (HasText(command.IdToken)) return command.IdToken!;
        if (!HasText(command.AuthorizationCode)) throw new ProviderValidationException();

        return await ExchangeAuthorizationCodeAsync(command, configuration, cancellationToken);
    }

    private async Task<string> ExchangeAuthorizationCodeAsync(
        SocialSignInCommand command,
        ProviderConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (!HasText(configuration.TokenUri)) throw new ProviderConfigurationException();

        var clientSecret = CreateClientSecret(configuration);
        var formValues = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "authorization_code"),
            new("code", command.AuthorizationCode!),
            new("client_id", configuration.ClientId!),
            new("client_secret", clientSecret)
        };
        var redirectUri = HasText(command.RedirectUri) ? command.RedirectUri : configuration.RedirectUri;
        if (HasText(redirectUri)) formValues.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri!));

        using var request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenUri)
        {
            Content = new FormUrlEncodedContent(formValues)
        };
        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        if (!response.IsSuccessStatusCode) throw new ProviderValidationException();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var responseDocument = await JsonDocument.ParseAsync(
            responseStream,
            cancellationToken: cancellationToken);
        if (responseDocument.RootElement.ValueKind != JsonValueKind.Object ||
            !responseDocument.RootElement.TryGetProperty("id_token", out var idTokenElement) ||
            idTokenElement.ValueKind != JsonValueKind.String ||
            !HasText(idTokenElement.GetString()))
            throw new ProviderValidationException();

        return idTokenElement.GetString()!;
    }

    private string CreateClientSecret(ProviderConfiguration configuration)
    {
        if (configuration.Provider == SocialProvider.Google)
        {
            if (!HasText(configuration.ClientSecret)) throw new ProviderConfigurationException();
            return configuration.ClientSecret!;
        }

        if (!HasText(configuration.AppleTeamId) ||
            !HasText(configuration.AppleKeyId) ||
            !HasText(configuration.ApplePrivateKey))
            throw new ProviderConfigurationException();

        try
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(configuration.ApplePrivateKey);
            if (ecdsa.KeySize != 256) throw new ProviderConfigurationException();

            var now = DateTime.UtcNow;
            var signingKey = new ECDsaSecurityKey(ecdsa) { KeyId = configuration.AppleKeyId };
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration.AppleTeamId,
                Audience = AppleClientSecretAudience,
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sub, configuration.ClientId!)
                ]),
                IssuedAt = now,
                Expires = now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.EcdsaSha256)
            };
            return _tokenHandler.CreateToken(descriptor);
        }
        catch (ProviderConfigurationException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ProviderConfigurationException();
        }
    }

    private async Task<ProviderIdentity> ValidateIdTokenAsync(
        SocialSignInCommand command,
        ProviderConfiguration configuration,
        string idToken,
        CancellationToken cancellationToken)
    {
        var unvalidatedToken = _tokenHandler.ReadJsonWebToken(idToken);
        if (!string.Equals(unvalidatedToken.Alg, SecurityAlgorithms.RsaSha256, StringComparison.Ordinal) ||
            !HasText(unvalidatedToken.Kid))
            throw new ProviderValidationException();

        var signingKey = await GetSigningKeyAsync(
            configuration,
            unvalidatedToken.Kid,
            cancellationToken);
        var validationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            TryAllIssuerSigningKeys = false,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ValidateIssuer = true,
            ValidIssuers = GetValidIssuers(configuration),
            ValidateAudience = true,
            ValidAudience = configuration.ClientId,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        var validationResult = await _tokenHandler.ValidateTokenAsync(idToken, validationParameters);
        cancellationToken.ThrowIfCancellationRequested();
        if (!validationResult.IsValid || validationResult.SecurityToken is not JsonWebToken validatedToken)
            throw new ProviderValidationException();

        var subject = NormalizeText(validatedToken.Subject);
        if (subject is null) throw new ProviderValidationException();

        var nonce = GetNullableClaim(validatedToken, "nonce");
        if (HasText(command.Nonce) && !string.Equals(command.Nonce, nonce, StringComparison.Ordinal))
            throw new ProviderValidationException();

        if (validatedToken.TryGetClaim("email_verified", out var emailVerifiedClaim) &&
            (!bool.TryParse(emailVerifiedClaim.Value, out var emailVerified) || !emailVerified))
            throw new ProviderValidationException();

        return new ProviderIdentity(
            command.Provider,
            subject,
            NormalizeEmail(GetNullableClaim(validatedToken, "email")),
            GetNullableClaim(validatedToken, "name"),
            GetNullableClaim(validatedToken, "given_name"),
            GetNullableClaim(validatedToken, "family_name"),
            idToken);
    }

    private async Task<SecurityKey> GetSigningKeyAsync(
        ProviderConfiguration configuration,
        string keyId,
        CancellationToken cancellationToken)
    {
        var cachedJwks = await LoadJwksAsync(configuration, false, null, cancellationToken);
        if (cachedJwks.Keys.TryGetValue(keyId, out var signingKey)) return signingKey;

        var refreshedJwks = await LoadJwksAsync(configuration, true, cachedJwks, cancellationToken);
        if (refreshedJwks.Keys.TryGetValue(keyId, out signingKey)) return signingKey;

        throw new ProviderValidationException();
    }

    private async Task<CachedJwks> LoadJwksAsync(
        ProviderConfiguration configuration,
        bool forceRefresh,
        CachedJwks? observedCache,
        CancellationToken cancellationToken)
    {
        if (!forceRefresh && TryGetCurrentJwks(configuration.Provider, out var currentJwks))
            return currentJwks;

        var refreshLock = _jwksRefreshLocks.GetOrAdd(configuration.Provider, _ => new SemaphoreSlim(1, 1));
        await refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (TryGetCurrentJwks(configuration.Provider, out currentJwks) &&
                (!forceRefresh || !ReferenceEquals(currentJwks, observedCache)))
                return currentJwks;

            var fetchedJwks = await FetchJwksAsync(configuration, cancellationToken);
            _jwksCache[configuration.Provider] = fetchedJwks;
            return fetchedJwks;
        }
        finally
        {
            refreshLock.Release();
        }
    }

    private bool TryGetCurrentJwks(SocialProvider provider, out CachedJwks cachedJwks)
    {
        if (_jwksCache.TryGetValue(provider, out var candidate) && candidate.ExpiresAt > DateTimeOffset.UtcNow)
        {
            cachedJwks = candidate;
            return true;
        }

        cachedJwks = null!;
        return false;
    }

    private async Task<CachedJwks> FetchJwksAsync(
        ProviderConfiguration configuration,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, configuration.JwksUri);
        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        if (!response.IsSuccessStatusCode) throw new ProviderValidationException();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("keys", out var keysElement) ||
            keysElement.ValueKind != JsonValueKind.Array)
            throw new ProviderValidationException();

        var keys = new Dictionary<string, SecurityKey>(StringComparer.Ordinal);
        foreach (var keyElement in keysElement.EnumerateArray())
        {
            if (keyElement.ValueKind != JsonValueKind.Object ||
                !string.Equals(GetString(keyElement, "kty"), "RSA", StringComparison.Ordinal) ||
                !IsSupportedOptionalValue(GetString(keyElement, "alg"), SecurityAlgorithms.RsaSha256) ||
                !IsSupportedOptionalValue(GetString(keyElement, "use"), "sig"))
                continue;

            var keyId = GetString(keyElement, "kid");
            var modulus = GetString(keyElement, "n");
            var exponent = GetString(keyElement, "e");
            if (!HasText(keyId) || !HasText(modulus) || !HasText(exponent)) continue;

            try
            {
                keys[keyId!] = new RsaSecurityKey(new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(modulus),
                    Exponent = Base64UrlEncoder.DecodeBytes(exponent)
                })
                {
                    KeyId = keyId
                };
            }
            catch (Exception exception) when (exception is ArgumentException or FormatException)
            {
                // Ignore malformed keys while retaining other valid keys from the provider document.
            }
        }

        if (keys.Count == 0) throw new ProviderValidationException();
        return new CachedJwks(keys, DateTimeOffset.UtcNow.Add(JwksCacheTtl));
    }

    private static IEnumerable<string> GetValidIssuers(ProviderConfiguration configuration) =>
        configuration.Provider == SocialProvider.Google
            ? new[] { configuration.Issuer, "accounts.google.com" }.Distinct(StringComparer.Ordinal)
            : [configuration.Issuer];

    private static string? GetNullableClaim(JsonWebToken token, string claimName) =>
        token.TryGetClaim(claimName, out var claim) ? NormalizeText(claim.Value) : null;

    private static string? NormalizeEmail(string? value) =>
        NormalizeText(value)?.ToLowerInvariant();

    private static string? NormalizeText(string? value) =>
        HasText(value) ? value!.Trim() : null;

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static bool IsSupportedOptionalValue(string? actual, string expected) =>
        actual is null || string.Equals(actual, expected, StringComparison.Ordinal);

    private static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);

    private sealed record ProviderConfiguration(
        SocialProvider Provider,
        string? ClientId,
        string? ClientSecret,
        string? RedirectUri,
        string TokenUri,
        string JwksUri,
        string Issuer,
        string? AppleTeamId,
        string? AppleKeyId,
        string? ApplePrivateKey);

    private sealed record CachedJwks(
        IReadOnlyDictionary<string, SecurityKey> Keys,
        DateTimeOffset ExpiresAt);

    private sealed class ProviderConfigurationException : Exception;

    private sealed class ProviderValidationException : Exception;
}
