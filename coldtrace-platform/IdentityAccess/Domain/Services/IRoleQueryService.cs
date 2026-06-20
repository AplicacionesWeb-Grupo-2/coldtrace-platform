using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;

namespace ColdTrace.Platform.IdentityAccess.Domain.Services;

/// <summary>
///     Application service contract for role queries.
/// </summary>
public interface IRoleQueryService
{
    /// <summary>
    ///     Handles querying all roles.
    /// </summary>
    /// <param name="query">Query object.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Roles with permission metadata.</returns>
    Task<IEnumerable<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default);
}
