namespace ColdTrace.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     Password reset metadata retained by the IAM bounded context.
/// </summary>
public class PasswordResetRequest
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected PasswordResetRequest()
    {
        Email = string.Empty;
        TokenHash = string.Empty;
    }

    /// <summary>
    ///     Creates safe password reset metadata for a known user.
    /// </summary>
    public PasswordResetRequest(
        string email,
        int userId,
        string tokenHash,
        DateTimeOffset requestedAt,
        DateTimeOffset expiresAt)
    {
        Email = RequireEmail(email);
        UserId = RequirePositive(userId, nameof(userId));
        TokenHash = RequireNonBlank(tokenHash, nameof(tokenHash));
        if (expiresAt <= requestedAt) throw new ArgumentException("Expiration must be after the request timestamp.");
        RequestedAt = requestedAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    ///     Gets the server-generated request identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the normalized email associated with the request.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    ///     Gets the user identifier without exposing user data through this aggregate.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    ///     Gets the SHA-256 token hash. The raw token is never persisted.
    /// </summary>
    public string TokenHash { get; private set; }

    /// <summary>
    ///     Gets the request timestamp.
    /// </summary>
    public DateTimeOffset RequestedAt { get; private set; }

    /// <summary>
    ///     Gets the expiration timestamp.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    ///     Gets the optional consumption timestamp reserved for a future confirmation flow.
    /// </summary>
    public DateTimeOffset? ConsumedAt { get; private set; }

    private static string RequireEmail(string? email)
    {
        var normalized = RequireNonBlank(email, nameof(email)).ToLowerInvariant();
        if (!normalized.Contains('@')) throw new ArgumentException("Email is invalid.");
        return normalized;
    }

    private static string RequireNonBlank(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.");
        return value.Trim();
    }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
