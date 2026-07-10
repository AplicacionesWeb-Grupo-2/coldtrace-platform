using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices.Social;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.IdentityAccess.Infrastructure.OAuth.Configuration;
using ColdTrace.Platform.IdentityAccess.Infrastructure.OAuth.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class OidcExternalIdentityProviderServiceTests
{
    private const string ClientId = "coldtrace-web-client";
    private const string Issuer = "https://provider.test";
    private const string JwksUri = "https://provider.test/.well-known/jwks.json";
    private const string TokenUri = "https://provider.test/oauth/token";

    [Theory]
    [InlineData(SocialProvider.Google)]
    [InlineData(SocialProvider.Apple)]
    public async Task ValidateAsync_WithValidRs256Token_ReturnsNormalizedProviderIdentity(
        SocialProvider provider)
    {
        using var key = new RsaOidcKey("valid-key");
        var idToken = key.CreateIdToken(
            email: " Operator@ColdTrace.Test ",
            fullName: " Cold Operator ",
            givenName: " Cold ",
            familyName: " Operator ",
            nonce: "nonce-123");
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);
        var command = new SocialSignInCommand(provider, idToken, null, null, "nonce-123");

        var result = await context.Service.ValidateAsync(command);

        var success = Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(result);
        Assert.Equal(provider, success.Value.Provider);
        Assert.Equal("provider-subject", success.Value.Subject);
        Assert.Equal("operator@coldtrace.test", success.Value.Email);
        Assert.Equal("Cold Operator", success.Value.FullName);
        Assert.Equal("Cold", success.Value.GivenName);
        Assert.Equal("Operator", success.Value.FamilyName);
        Assert.Equal(idToken, success.Value.IdToken);
        Assert.Equal(OidcExternalIdentityProviderService.HttpClientName, context.Factory.RequestedName);
        Assert.Equal(1, handler.JwksRequestCount);
        Assert.Equal(0, handler.TokenRequestCount);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidIssuer_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("issuer-key");
        var idToken = key.CreateIdToken(issuer: "https://unexpected-issuer.test");
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidAudience_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("audience-key");
        var idToken = key.CreateIdToken(audience: "another-client");
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithMismatchedNonce_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("nonce-key");
        var idToken = key.CreateIdToken(nonce: "provider-nonce");
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);
        var command = new SocialSignInCommand(
            SocialProvider.Google,
            idToken,
            null,
            null,
            "different-nonce");

        var result = await context.Service.ValidateAsync(command);

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithUnverifiedEmail_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("email-key");
        var idToken = key.CreateIdToken(emailVerified: false);
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidSignature_ReturnsProviderValidationFailed()
    {
        using var trustedKey = new RsaOidcKey("shared-key-id");
        using var untrustedKey = new RsaOidcKey("shared-key-id");
        var idToken = untrustedKey.CreateIdToken();
        using var handler = new ProviderHttpMessageHandler(_ => trustedKey.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithExpiredToken_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("expired-key");
        var idToken = key.CreateIdToken(expires: DateTime.UtcNow.AddMinutes(-1));
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithoutSubject_ReturnsProviderValidationFailed()
    {
        using var key = new RsaOidcKey("subject-key");
        var idToken = key.CreateIdToken(subject: null);
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        AssertProviderValidationFailed(result);
    }

    [Fact]
    public async Task ValidateAsync_WithoutClientId_ReturnsProviderConfigurationMissingWithoutHttpCall()
    {
        var options = CreateOptions();
        options.Google.ClientId = null;
        using var handler = new ProviderHttpMessageHandler(_ => "{}");
        using var context = CreateService(options, handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand("unread-token"));

        var failure = Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Failure>(result);
        Assert.Equal("SOCIAL_PROVIDER_CONFIGURATION_MISSING", failure.Error.Code);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task ValidateAsync_WithGoogleAuthorizationCode_ExchangesCodeServerSide()
    {
        using var key = new RsaOidcKey("google-code-key");
        var idToken = key.CreateIdToken();
        var options = CreateOptions();
        options.Google.ClientSecret = "google-client-secret";
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson, idToken);
        using var context = CreateService(options, handler);
        var command = new SocialSignInCommand(
            SocialProvider.Google,
            null,
            "authorization+code/value",
            "https://client.test/callback?flow=social",
            null);

        var result = await context.Service.ValidateAsync(command);

        Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(result);
        var request = Assert.Single(handler.TokenRequests);
        Assert.Equal("authorization_code", request["grant_type"]);
        Assert.Equal("authorization+code/value", request["code"]);
        Assert.Equal(ClientId, request["client_id"]);
        Assert.Equal("google-client-secret", request["client_secret"]);
        Assert.Equal("https://client.test/callback?flow=social", request["redirect_uri"]);
        Assert.Equal(1, handler.JwksRequestCount);
    }

    [Fact]
    public async Task ValidateAsync_WithAppleAuthorizationCode_GeneratesValidEs256ClientSecret()
    {
        using var oidcKey = new RsaOidcKey("apple-code-key");
        using var applePrivateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var idToken = oidcKey.CreateIdToken();
        var options = CreateOptions();
        options.Apple.TeamId = "APPLE_TEAM_ID";
        options.Apple.KeyId = "APPLE_KEY_ID";
        options.Apple.PrivateKey = applePrivateKey.ExportPkcs8PrivateKeyPem()
            .Replace("\n", "\\n", StringComparison.Ordinal);
        using var handler = new ProviderHttpMessageHandler(_ => oidcKey.JwksJson, idToken);
        using var context = CreateService(options, handler);
        var command = new SocialSignInCommand(
            SocialProvider.Apple,
            null,
            "apple-authorization-code",
            null,
            null);

        var result = await context.Service.ValidateAsync(command);

        Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(result);
        var request = Assert.Single(handler.TokenRequests);
        Assert.Equal("apple-authorization-code", request["code"]);
        var clientSecret = request["client_secret"];
        var parsedClientSecret = new JsonWebTokenHandler().ReadJsonWebToken(clientSecret);
        Assert.Equal(SecurityAlgorithms.EcdsaSha256, parsedClientSecret.Alg);
        Assert.Equal("APPLE_KEY_ID", parsedClientSecret.Kid);
        Assert.Equal("APPLE_TEAM_ID", parsedClientSecret.Issuer);
        Assert.Equal(ClientId, parsedClientSecret.Subject);

        using var applePublicKey = ECDsa.Create(applePrivateKey.ExportParameters(false));
        var secretValidation = await new JsonWebTokenHandler().ValidateTokenAsync(
            clientSecret,
            new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new ECDsaSecurityKey(applePublicKey) { KeyId = "APPLE_KEY_ID" },
                ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
                ValidateIssuer = true,
                ValidIssuer = "APPLE_TEAM_ID",
                ValidateAudience = true,
                ValidAudience = "https://appleid.apple.com",
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            });
        Assert.True(secretValidation.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ReusesJwksWithinCacheWindow()
    {
        using var key = new RsaOidcKey("cache-key");
        var idToken = key.CreateIdToken();
        using var handler = new ProviderHttpMessageHandler(_ => key.JwksJson);
        using var context = CreateService(CreateOptions(), handler);
        var command = CreateGoogleCommand(idToken);

        var firstResult = await context.Service.ValidateAsync(command);
        var secondResult = await context.Service.ValidateAsync(command);

        Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(firstResult);
        Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(secondResult);
        Assert.Equal(1, handler.JwksRequestCount);
    }

    [Fact]
    public async Task ValidateAsync_WhenKidIsMissingFromCachedJwks_RefreshesJwksOnce()
    {
        using var oldKey = new RsaOidcKey("old-key");
        using var currentKey = new RsaOidcKey("rotated-key");
        var idToken = currentKey.CreateIdToken();
        using var handler = new ProviderHttpMessageHandler(requestNumber =>
            requestNumber == 1 ? oldKey.JwksJson : currentKey.JwksJson);
        using var context = CreateService(CreateOptions(), handler);

        var result = await context.Service.ValidateAsync(CreateGoogleCommand(idToken));

        Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Success>(result);
        Assert.Equal(2, handler.JwksRequestCount);
    }

    [Fact]
    public void ExpandEnvironmentVariables_ExpandsSecretsAndRestoresProviderDefaults()
    {
        var environmentVariable = $"COLDTRACE_SOCIAL_TEST_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(environmentVariable, " configured-client ");
        try
        {
            var options = new SocialAuthenticationOptions
            {
                Google = new GoogleSocialAuthenticationOptions
                {
                    ClientId = $"%{environmentVariable}%",
                    TokenUri = "%MISSING_SOCIAL_TOKEN_URI%",
                    JwksUri = "$MISSING_SOCIAL_JWKS_URI",
                    Issuer = " "
                }
            };

            options.ExpandEnvironmentVariables();

            Assert.Equal("configured-client", options.Google.ClientId);
            Assert.Equal(GoogleSocialAuthenticationOptions.DefaultTokenUri, options.Google.TokenUri);
            Assert.Equal(GoogleSocialAuthenticationOptions.DefaultJwksUri, options.Google.JwksUri);
            Assert.Equal(GoogleSocialAuthenticationOptions.DefaultIssuer, options.Google.Issuer);
            Assert.Equal(AppleSocialAuthenticationOptions.DefaultTokenUri, options.Apple.TokenUri);
        }
        finally
        {
            Environment.SetEnvironmentVariable(environmentVariable, null);
        }
    }

    private static SocialSignInCommand CreateGoogleCommand(string idToken) =>
        new(SocialProvider.Google, idToken, null, null, null);

    private static SocialAuthenticationOptions CreateOptions() =>
        new()
        {
            Google = new GoogleSocialAuthenticationOptions
            {
                ClientId = ClientId,
                RedirectUri = "https://client.test/google/callback",
                TokenUri = TokenUri,
                JwksUri = JwksUri,
                Issuer = Issuer
            },
            Apple = new AppleSocialAuthenticationOptions
            {
                ClientId = ClientId,
                RedirectUri = "https://client.test/apple/callback",
                TokenUri = TokenUri,
                JwksUri = JwksUri,
                Issuer = Issuer
            }
        };

    private static ServiceContext CreateService(
        SocialAuthenticationOptions options,
        ProviderHttpMessageHandler handler) =>
        new(options, handler);

    private static void AssertProviderValidationFailed(
        Result<ProviderIdentity, SocialAuthenticationError> result)
    {
        var failure = Assert.IsType<Result<ProviderIdentity, SocialAuthenticationError>.Failure>(result);
        Assert.Equal("PROVIDER_VALIDATION_FAILED", failure.Error.Code);
    }

    private sealed class ServiceContext : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ServiceContext(
            SocialAuthenticationOptions options,
            ProviderHttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler, false);
            Factory = new TestHttpClientFactory(_httpClient);
            Service = new OidcExternalIdentityProviderService(
                Factory,
                Options.Create(options),
                NullLogger<OidcExternalIdentityProviderService>.Instance);
        }

        public TestHttpClientFactory Factory { get; }

        public OidcExternalIdentityProviderService Service { get; }

        public void Dispose() => _httpClient.Dispose();
    }

    private sealed class TestHttpClientFactory(HttpClient httpClient) : IHttpClientFactory
    {
        public string? RequestedName { get; private set; }

        public HttpClient CreateClient(string name)
        {
            RequestedName = name;
            return httpClient;
        }
    }

    private sealed class ProviderHttpMessageHandler(
        Func<int, string> jwksResponse,
        string? exchangedIdToken = null) : HttpMessageHandler
    {
        public int JwksRequestCount { get; private set; }

        public int TokenRequestCount { get; private set; }

        public int RequestCount => JwksRequestCount + TokenRequestCount;

        public List<IReadOnlyDictionary<string, string>> TokenRequests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get && request.RequestUri == new Uri(JwksUri))
            {
                JwksRequestCount++;
                return JsonResponse(jwksResponse(JwksRequestCount));
            }

            if (request.Method == HttpMethod.Post && request.RequestUri == new Uri(TokenUri))
            {
                TokenRequestCount++;
                var body = await request.Content!.ReadAsStringAsync(cancellationToken);
                TokenRequests.Add(ParseForm(body));
                return exchangedIdToken is null
                    ? new HttpResponseMessage(HttpStatusCode.BadRequest)
                    : JsonResponse(JsonSerializer.Serialize(new { id_token = exchangedIdToken }));
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private static HttpResponseMessage JsonResponse(string body) =>
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

        private static IReadOnlyDictionary<string, string> ParseForm(string body) =>
            body.Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Split('=', 2))
                .ToDictionary(
                    pair => DecodeFormValue(pair[0]),
                    pair => DecodeFormValue(pair.Length == 2 ? pair[1] : string.Empty),
                    StringComparer.Ordinal);

        private static string DecodeFormValue(string value) =>
            Uri.UnescapeDataString(value.Replace('+', ' '));
    }

    private sealed class RsaOidcKey : IDisposable
    {
        private readonly RSA _rsa = RSA.Create(2048);

        public RsaOidcKey(string keyId)
        {
            KeyId = keyId;
            var parameters = _rsa.ExportParameters(false);
            JwksJson = JsonSerializer.Serialize(new
            {
                keys = new[]
                {
                    new
                    {
                        kty = "RSA",
                        kid = keyId,
                        use = "sig",
                        alg = SecurityAlgorithms.RsaSha256,
                        n = Base64UrlEncoder.Encode(parameters.Modulus),
                        e = Base64UrlEncoder.Encode(parameters.Exponent)
                    }
                }
            });
        }

        public string KeyId { get; }

        public string JwksJson { get; }

        public string CreateIdToken(
            string? issuer = Issuer,
            string? audience = ClientId,
            string? subject = "provider-subject",
            string? email = "operator@coldtrace.test",
            bool? emailVerified = true,
            string? fullName = "Cold Operator",
            string? givenName = "Cold",
            string? familyName = "Operator",
            string? nonce = null,
            DateTime? expires = null)
        {
            var now = DateTime.UtcNow;
            var expiration = expires ?? now.AddMinutes(5);
            var issuedAt = expiration <= now ? expiration.AddMinutes(-5) : now.AddMinutes(-1);
            var claims = new Dictionary<string, object>();
            AddClaim(claims, JwtRegisteredClaimNames.Sub, subject);
            AddClaim(claims, "email", email);
            if (emailVerified.HasValue) claims["email_verified"] = emailVerified.Value;
            AddClaim(claims, "name", fullName);
            AddClaim(claims, "given_name", givenName);
            AddClaim(claims, "family_name", familyName);
            AddClaim(claims, "nonce", nonce);

            return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = claims,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = expiration,
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(_rsa) { KeyId = KeyId },
                    SecurityAlgorithms.RsaSha256)
            });
        }

        public void Dispose() => _rsa.Dispose();

        private static void AddClaim(IDictionary<string, object> claims, string name, string? value)
        {
            if (value is not null) claims[name] = value;
        }
    }
}
