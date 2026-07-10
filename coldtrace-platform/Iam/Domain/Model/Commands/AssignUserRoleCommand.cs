namespace ColdTrace.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Command for assigning a role to an existing organization user.
/// </summary>
public record AssignUserRoleCommand
{
    /// <summary>
    ///     Creates a command with validated user role assignment identifiers.
    /// </summary>
    public AssignUserRoleCommand(int organizationId, int userId, int roleId)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        UserId = RequirePositive(userId, nameof(userId));
        RoleId = RequirePositive(roleId, nameof(roleId));
    }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; init; }

    /// <summary>
    ///     Gets the user identifier.
    /// </summary>
    public int UserId { get; init; }

    /// <summary>
    ///     Gets the target role identifier.
    /// </summary>
    public int RoleId { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
