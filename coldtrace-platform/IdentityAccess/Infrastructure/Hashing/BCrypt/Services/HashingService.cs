using ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
///     BCrypt password hashing service.
/// </summary>
public class HashingService : IHashingService
{
    /// <inheritdoc />
    public string HashPassword(string password) => BCryptNet.HashPassword(password);

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash) => BCryptNet.Verify(password, passwordHash);
}
