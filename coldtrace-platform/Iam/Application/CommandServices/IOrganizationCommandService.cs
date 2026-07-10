using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.CommandServices;

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
