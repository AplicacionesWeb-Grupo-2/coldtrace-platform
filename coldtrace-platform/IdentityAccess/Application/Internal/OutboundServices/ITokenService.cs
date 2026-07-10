using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service for JWT issuance and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Generates a signed JWT for a user.
    /// </summary>
    /// <param name="user">Authenticated user.</param>
    /// <returns>The signed token.</returns>
    string GenerateToken(User user);

    /// <summary>
    ///     Validates a JWT and returns its user identifier.
    /// </summary>
    /// <param name="token">JWT value.</param>
    /// <returns>The user identifier when the token is valid; otherwise null.</returns>
    Task<int?> ValidateToken(string token);
}
