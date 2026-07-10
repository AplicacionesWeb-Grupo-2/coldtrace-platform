using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;

namespace ColdTrace.Platform.Iam.Application.QueryServices;

/// <summary>
///     Application service contract for organization query operations.
/// </summary>
public interface IOrganizationQueryService
{
    /// <summary>
    ///     Handles the all-organizations query.
    /// </summary>
    /// <param name="query">The all-organizations query.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A collection of organizations, possibly empty.</returns>
    Task<IEnumerable<Organization>> Handle(
        GetAllOrganizationsQuery query,
        CancellationToken cancellationToken = default);
}
