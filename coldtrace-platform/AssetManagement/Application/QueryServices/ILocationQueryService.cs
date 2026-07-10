using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.QueryServices;

/// <summary>
///     Location query application service contract.
/// </summary>
public interface ILocationQueryService
{
    /// <summary>
    ///     Retrieves locations by organization.
    /// </summary>
    /// <param name="query">Query containing the organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Locations for the organization or a query error.</returns>
    Task<Result<IEnumerable<Location>, GetLocationsByOrganizationError>> Handle(
        GetLocationsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves one location by id and organization.
    /// </summary>
    /// <param name="query">Query containing organization and location identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The location or a query error.</returns>
    Task<Result<Location, GetLocationByIdAndOrganizationError>> Handle(
        GetLocationByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
