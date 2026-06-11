using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Services;

/// <summary>
///     Application service contract for user commands.
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    ///     Handles user creation.
    /// </summary>
    /// <param name="command">Command containing user data and references.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with the created user or a creation error.</returns>
    Task<Result<User, CreateUserError>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default);
}
