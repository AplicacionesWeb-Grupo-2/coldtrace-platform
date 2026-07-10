namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

/// <summary>
///     Command for signing up an organization and its first user.
/// </summary>
public record CreateOrganizationSignUpCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized sign-up data.
    /// </summary>
    public CreateOrganizationSignUpCommand(
        string legalName,
        string commercialName,
        string? taxId,
        string contactEmail,
        string firstName,
        string? lastName,
        string email,
        string password)
    {
        LegalName = RequireNonBlank(legalName);
        CommercialName = RequireNonBlank(commercialName);
        TaxId = NormalizeOptional(taxId);
        ContactEmail = RequireValidEmail(contactEmail);
        FirstName = RequireNonBlank(firstName);
        LastName = lastName?.Trim() ?? string.Empty;
        Email = RequireValidEmail(email);
        Password = RequireValidPassword(password);
    }

    /// <summary>
    ///     Gets the organization legal name.
    /// </summary>
    public string LegalName { get; init; }

    /// <summary>
    ///     Gets the organization commercial name.
    /// </summary>
    public string CommercialName { get; init; }

    /// <summary>
    ///     Gets the optional organization tax identifier.
    /// </summary>
    public string? TaxId { get; init; }

    /// <summary>
    ///     Gets the organization contact email.
    /// </summary>
    public string ContactEmail { get; init; }

    /// <summary>
    ///     Gets the first user first name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    ///     Gets the first user last name.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    ///     Gets the first user email address.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    ///     Gets the first user raw password used to create the password hash.
    /// </summary>
    public string Password { get; init; }

    /// <summary>
    ///     Converts this sign-up command into an organization creation command.
    /// </summary>
    /// <returns>An organization creation command.</returns>
    public CreateOrganizationCommand ToCreateOrganizationCommand() =>
        new(LegalName, CommercialName, TaxId, ContactEmail);

    /// <summary>
    ///     Converts this sign-up command into a user creation command.
    /// </summary>
    /// <returns>A user creation command.</returns>
    public CreateUserCommand ToCreateUserCommand() => new(FirstName, LastName, Email, Password);

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string RequireValidEmail(string? value)
    {
        var normalized = RequireNonBlank(value).ToLowerInvariant();
        if (!normalized.Contains('@')) throw new ArgumentException("Email is invalid.");
        return normalized;
    }

    private static string RequireValidPassword(string? value)
    {
        var password = RequireNonBlank(value);
        if (password.Length < 8) throw new ArgumentException("Password must have at least 8 characters.");
        return password;
    }
}
