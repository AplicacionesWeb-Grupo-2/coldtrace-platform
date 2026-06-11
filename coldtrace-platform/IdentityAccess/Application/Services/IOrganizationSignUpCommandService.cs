using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Services;

/// <summary>
///     Application service contract for organization sign-up commands.
/// </summary>
public interface IOrganizationSignUpCommandService
{
    /// <summary>
    ///     Handles organization sign-up by creating the organization and its first user.
    /// </summary>
    /// <param name="command">Command containing organization and first user data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with the created aggregates or a sign-up error.</returns>
    Task<Result<OrganizationSignUpResult, CreateOrganizationSignUpError>> Handle(
        CreateOrganizationSignUpCommand command,
        CancellationToken cancellationToken = default);
}
