using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;

/// <summary>
///     User aggregate for the identity access context.
/// </summary>
public class User
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    /// <summary>
    ///     Creates a user from a create command and aggregate references.
    /// </summary>
    /// <param name="command">Command containing user identity data.</param>
    /// <param name="organization">Organization that owns the user.</param>
    /// <param name="role">Role assigned to the user.</param>
    public User(CreateUserCommand command, Organization organization, Role role)
    {
        FirstName = command.FirstName;
        LastName = command.LastName;
        Email = command.Email;
        Organization = organization;
        Role = role;
    }

    /// <summary>
    ///     Creates a user from a create command with validated identifiers.
    /// </summary>
    /// <param name="command">Command containing user identity data and references.</param>
    public User(CreateUserCommand command)
    {
        FirstName = command.FirstName;
        LastName = command.LastName;
        Email = command.Email;
        OrganizationId = command.OrganizationId;
        RoleId = command.RoleId;
    }

    /// <summary>
    ///     Gets the server-generated user identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the optional generated user code.
    /// </summary>
    public string? Uuid { get; private set; }

    /// <summary>
    ///     Gets the optional organization-scoped user identifier.
    /// </summary>
    public int? OrganizationUserId { get; private set; }

    /// <summary>
    ///     Gets the user first name.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    ///     Gets the user last name.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    ///     Gets the user email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the assigned role identifier.
    /// </summary>
    public int RoleId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the assigned role.
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    ///     Assigns a role to the user.
    /// </summary>
    /// <param name="command">Command containing the target role identifier.</param>
    public void AssignRole(AssignUserRoleCommand command)
    {
        RoleId = command.RoleId;
    }
}
