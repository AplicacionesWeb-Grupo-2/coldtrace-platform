namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

/// <summary>
///     Command for deleting a user inside an organization.
/// </summary>
public record DeleteUserCommand
{
    /// <summary>
    ///     Initializes a user deletion command.
    /// </summary>
    /// <param name="organizationId">Organization identifier that scopes the user.</param>
    /// <param name="userId">User identifier to delete.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier is not positive.</exception>
    public DeleteUserCommand(int organizationId, int userId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));
        if (userId <= 0)
            throw new ArgumentException("User identifier must be positive.", nameof(userId));

        OrganizationId = organizationId;
        UserId = userId;
    }

    /// <summary>
    ///     Organization identifier that scopes the user.
    /// </summary>
    public int OrganizationId { get; }

    /// <summary>
    ///     User identifier to delete.
    /// </summary>
    public int UserId { get; }
}
