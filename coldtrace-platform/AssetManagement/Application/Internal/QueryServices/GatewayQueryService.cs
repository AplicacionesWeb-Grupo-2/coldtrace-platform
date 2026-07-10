using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Application.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.QueryServices;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for gateway query operations.
/// </summary>
public class GatewayQueryService(
    IGatewayRepository gatewayRepository,
    IIamContextFacade iamContextFacade,
    ILogger<GatewayQueryService> logger)
    : IGatewayQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>> Handle(
        GetGatewaysByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for gateway query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>.Failure(
                GetGatewaysByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var gateways = await gatewayRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>.Success(gateways);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying gateways for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>.Failure(
                GetGatewaysByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Gateway, GetGatewayByIdAndOrganizationError>> Handle(
        GetGatewayByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning(
                "Organization not found for gateway by id query: {OrganizationId}",
                query.OrganizationId);
            return new Result<Gateway, GetGatewayByIdAndOrganizationError>.Failure(
                GetGatewayByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var gateway = await gatewayRepository.FindByIdAndOrganizationIdAsync(
                query.GatewayId,
                query.OrganizationId,
                cancellationToken);
            if (gateway is null)
            {
                logger.LogWarning(
                    "Gateway not found for organization query: {OrganizationId} {GatewayId}",
                    query.OrganizationId,
                    query.GatewayId);
                return new Result<Gateway, GetGatewayByIdAndOrganizationError>.Failure(
                    GetGatewayByIdAndOrganizationError.GatewayNotFound);
            }

            return new Result<Gateway, GetGatewayByIdAndOrganizationError>.Success(gateway);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error querying gateway {GatewayId} for organization {OrganizationId}",
                query.GatewayId,
                query.OrganizationId);
            return new Result<Gateway, GetGatewayByIdAndOrganizationError>.Failure(
                GetGatewayByIdAndOrganizationError.UnexpectedError);
        }
    }
}
