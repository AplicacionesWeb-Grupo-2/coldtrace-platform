using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.MaintenanceManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for technical service request query operations.
/// </summary>
public class TechnicalServiceRequestQueryService(
    ITechnicalServiceRequestRepository technicalServiceRequestRepository,
    IOrganizationRepository organizationRepository,
    ILogger<TechnicalServiceRequestQueryService> logger)
    : ITechnicalServiceRequestQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<TechnicalServiceRequest>, GetTechnicalServiceRequestsByOrganizationError>>
        Handle(
            GetTechnicalServiceRequestsByOrganizationIdQuery query,
            CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for technical service requests query: {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<TechnicalServiceRequest>,
                GetTechnicalServiceRequestsByOrganizationError>.Failure(
                GetTechnicalServiceRequestsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var requests =
                await technicalServiceRequestRepository.FindAllByOrganizationIdAsync(query.OrganizationId,
                    cancellationToken);
            return
                new Result<IEnumerable<TechnicalServiceRequest>,
                    GetTechnicalServiceRequestsByOrganizationError>.Success(requests);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying technical service requests for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<TechnicalServiceRequest>,
                GetTechnicalServiceRequestsByOrganizationError>.Failure(
                GetTechnicalServiceRequestsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>> Handle(
        GetTechnicalServiceRequestByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for technical service request by id query: {OrganizationId}",
                query.OrganizationId);
            return new Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Failure(
                GetTechnicalServiceRequestByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var request = await technicalServiceRequestRepository.FindByIdAndOrganizationIdAsync(
                query.TechnicalServiceRequestId, query.OrganizationId, cancellationToken);
            if (request is null)
            {
                logger.LogWarning(
                    "Technical service request not found: org={OrganizationId} id={Id}",
                    query.OrganizationId, query.TechnicalServiceRequestId);
                return
                    new Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Failure(
                        GetTechnicalServiceRequestByIdAndOrganizationError.TechnicalServiceRequestNotFound);
            }

            return new Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Success(
                request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying technical service request {Id} for organization {OrganizationId}",
                query.TechnicalServiceRequestId, query.OrganizationId);
            return new Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Failure(
                GetTechnicalServiceRequestByIdAndOrganizationError.UnexpectedError);
        }
    }
}
