namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization-scoped user.
/// </summary>
public record CreateUserCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized user data.
    /// </summary>
    public CreateUserCommand(string firstName, string? lastName, string email)
    {
        FirstName = RequireNonBlank(firstName);
        LastName = lastName?.Trim() ?? string.Empty;
        Email = RequireValidEmail(email);
    }

    /// <summary>
    ///     Gets the user first name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    ///     Gets the user last name.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    ///     Gets the user email address.
    /// </summary>
    public string Email { get; init; }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }

    private static string RequireValidEmail(string? value)
    {
        var normalized = RequireNonBlank(value).ToLowerInvariant();
        if (!normalized.Contains('@')) throw new ArgumentException("Email is invalid.");
        return normalized;
    }
}
