using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Services;

/// <summary>
///     Application service contract for user queries.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    ///     Handles querying users by organization.
    /// </summary>
    /// <param name="query">Query containing the organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result with users or a query error.</returns>
    Task<Result<IEnumerable<User>, GetUsersByOrganizationError>> Handle(
        GetUsersByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
