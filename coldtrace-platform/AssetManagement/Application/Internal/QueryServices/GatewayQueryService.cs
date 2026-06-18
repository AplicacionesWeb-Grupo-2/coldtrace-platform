using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for gateway query operations.
/// </summary>
public class GatewayQueryService(
    IGatewayRepository gatewayRepository,
    IOrganizationRepository organizationRepository,
    ILogger<GatewayQueryService> logger)
    : IGatewayQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>> Handle(
        GetGatewaysByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
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
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
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
