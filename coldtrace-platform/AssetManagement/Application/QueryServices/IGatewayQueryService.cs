using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.QueryServices;

/// <summary>
///     Gateway query application service contract.
/// </summary>
public interface IGatewayQueryService
{
    /// <summary>
    ///     Retrieves gateways by organization.
    /// </summary>
    /// <param name="query">Query containing the organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Gateways for the organization or a query error.</returns>
    Task<Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>> Handle(
        GetGatewaysByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves one gateway by id and organization.
    /// </summary>
    /// <param name="query">Query containing organization and gateway identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The gateway or a query error.</returns>
    Task<Result<Gateway, GetGatewayByIdAndOrganizationError>> Handle(
        GetGatewayByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
