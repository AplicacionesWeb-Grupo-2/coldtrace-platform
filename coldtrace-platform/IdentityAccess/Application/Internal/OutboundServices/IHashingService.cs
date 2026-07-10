namespace ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service for password hashing operations.
/// </summary>
public interface IHashingService
{
    /// <summary>
    ///     Hashes a raw password for persistence.
    /// </summary>
    /// <param name="password">Raw password.</param>
    /// <returns>The encoded password hash.</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verifies a raw password against an encoded hash.
    /// </summary>
    /// <param name="password">Raw password.</param>
    /// <param name="passwordHash">Encoded password hash.</param>
    /// <returns>True when the password matches the hash.</returns>
    bool VerifyPassword(string password, string passwordHash);
}
