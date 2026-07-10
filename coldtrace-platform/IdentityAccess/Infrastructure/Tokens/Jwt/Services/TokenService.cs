using System.Globalization;
using System.Security.Claims;
using System.Text;
using ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Claims;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Services;

/// <summary>
///     JWT token service.
/// </summary>
public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    /// <inheritdoc />
    public string GenerateToken(User user)
    {
        var issuedAt = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Sid, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ColdTraceClaimTypes.OrganizationId,
                    user.OrganizationId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ColdTraceClaimTypes.RoleId, user.RoleId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Role, user.RoleId.ToString(CultureInfo.InvariantCulture))
            ]),
            IssuedAt = issuedAt,
            Expires = issuedAt.AddDays(GetExpirationDays()),
            SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    /// <inheritdoc />
    public async Task<int?> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        try
        {
            var validationResult = await new JsonWebTokenHandler().ValidateTokenAsync(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSigningKey(),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                });

            if (!validationResult.IsValid) return null;

            var userIdValue = validationResult.ClaimsIdentity?.FindFirst(ClaimTypes.Sid)?.Value;
            return int.TryParse(userIdValue, NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
                ? userId
                : null;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        if (string.IsNullOrWhiteSpace(_tokenSettings.Secret))
            throw new InvalidOperationException("JWT secret is not configured.");

        var key = Encoding.UTF8.GetBytes(_tokenSettings.Secret);
        if (key.Length < 32)
            throw new InvalidOperationException("JWT secret must contain at least 32 bytes.");

        return new SymmetricSecurityKey(key);
    }

    private int GetExpirationDays()
    {
        if (_tokenSettings.ExpirationDays <= 0)
            throw new InvalidOperationException("JWT expiration days must be positive.");

        return _tokenSettings.ExpirationDays;
    }
}
