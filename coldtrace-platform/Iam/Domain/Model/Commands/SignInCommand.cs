namespace ColdTrace.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Command for authenticating a user by email and password.
/// </summary>
public record SignInCommand
{
    /// <summary>
    ///     Creates a command with normalized credentials.
    /// </summary>
    public SignInCommand(string email, string password)
    {
        Email = RequireValidEmail(email);
        Password = RequireNonBlank(password);
    }

    /// <summary>
    ///     Gets the normalized user email address.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    ///     Gets the raw password supplied by the client.
    /// </summary>
    public string Password { get; init; }

    private static string RequireValidEmail(string? value)
    {
        var normalized = RequireNonBlank(value).ToLowerInvariant();
        if (!normalized.Contains('@')) throw new ArgumentException("Email is invalid.");
        return normalized;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
