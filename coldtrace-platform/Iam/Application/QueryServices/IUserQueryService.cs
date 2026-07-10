using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.QueryServices;

/// <summary>
///     Application service contract for user queries.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    ///     Handles querying a user by identifier.
    /// </summary>
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);

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
