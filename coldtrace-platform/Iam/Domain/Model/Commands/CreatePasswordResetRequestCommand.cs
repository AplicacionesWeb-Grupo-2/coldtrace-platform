namespace ColdTrace.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Command for requesting a password reset.
/// </summary>
public record CreatePasswordResetRequestCommand
{
    /// <summary>
    ///     Creates a command with a validated and normalized email.
    /// </summary>
    /// <param name="email">User email address.</param>
    public CreatePasswordResetRequestCommand(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
        Email = email.Trim().ToLowerInvariant();
        if (!Email.Contains('@')) throw new ArgumentException("Email is invalid.");
    }

    /// <summary>
    ///     Gets the normalized user email address.
    /// </summary>
    public string Email { get; init; }
}
