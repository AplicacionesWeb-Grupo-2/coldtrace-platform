namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization-scoped user.
/// </summary>
public record CreateUserCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized user data.
    /// </summary>
    public CreateUserCommand(string firstName, string? lastName, string email, string password) : this(
        firstName,
        lastName,
        email,
        password,
        0,
        0,
        false)
    {
    }

    /// <summary>
    ///     Creates a command with validated and normalized user data and references.
    /// </summary>
    public CreateUserCommand(
        string firstName,
        string? lastName,
        string email,
        string password,
        int organizationId,
        int roleId) : this(
        firstName,
        lastName,
        email,
        password,
        organizationId,
        roleId,
        true)
    {
    }

    private CreateUserCommand(
        string firstName,
        string? lastName,
        string email,
        string password,
        int organizationId,
        int roleId,
        bool validateReferences)
    {
        FirstName = RequireNonBlank(firstName);
        LastName = lastName?.Trim() ?? string.Empty;
        Email = RequireValidEmail(email);
        Password = RequireValidPassword(password);
        OrganizationId = validateReferences ? RequirePositive(organizationId, nameof(organizationId)) : organizationId;
        RoleId = validateReferences ? RequirePositive(roleId, nameof(roleId)) : roleId;
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

    /// <summary>
    ///     Gets the raw password used to create the password hash.
    /// </summary>
    public string Password { get; init; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the assigned role identifier.
    /// </summary>
    public int RoleId { get; init; }

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

    private static string RequireValidPassword(string? value)
    {
        var password = RequireNonBlank(value);
        if (password.Length < 8) throw new ArgumentException("Password must have at least 8 characters.");
        return password;
    }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
