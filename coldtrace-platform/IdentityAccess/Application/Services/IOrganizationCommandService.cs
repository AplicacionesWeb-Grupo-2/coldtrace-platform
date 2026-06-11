using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Services;

/// <summary>
///     Application service contract for organization command operations.
/// </summary>
public interface IOrganizationCommandService
{
    /// <summary>
    ///     Handles organization creation.
    /// </summary>
    /// <param name="command">The organization creation command.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created organization or a command failure.</returns>
    Task<Result<Organization, CreateOrganizationError>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken = default);
}
