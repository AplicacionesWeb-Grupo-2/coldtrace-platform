using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Domain.Services;

/// <summary>
///     Application service contract for user commands.
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    ///     Handles user authentication.
    /// </summary>
    /// <param name="command">Command containing user credentials.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with the authenticated user and token or an authentication error.</returns>
    Task<Result<AuthenticatedUserResult, AuthenticationError>> Handle(
        SignInCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles user creation.
    /// </summary>
    /// <param name="command">Command containing user data and references.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with the created user or a creation error.</returns>
    Task<Result<User, CreateUserError>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles assignment of a role to an existing organization user.
    /// </summary>
    /// <param name="command">Command containing organization, user and target role identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with the updated user or a role assignment error.</returns>
    Task<Result<User, AssignUserRoleError>> Handle(
        AssignUserRoleCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles deletion of an organization user.
    /// </summary>
    /// <param name="command">Command containing organization and user identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the command or a deletion error.</returns>
    Task<Result<DeleteUserCommand, DeleteUserError>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default);
}
