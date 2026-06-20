using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for location query operations.
/// </summary>
public class LocationQueryService(
    ILocationRepository locationRepository,
    IOrganizationRepository organizationRepository,
    ILogger<LocationQueryService> logger)
    : ILocationQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Location>, GetLocationsByOrganizationError>> Handle(
        GetLocationsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for location query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Location>, GetLocationsByOrganizationError>.Failure(
                GetLocationsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var locations = await locationRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Location>, GetLocationsByOrganizationError>.Success(locations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying locations for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Location>, GetLocationsByOrganizationError>.Failure(
                GetLocationsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Location, GetLocationByIdAndOrganizationError>> Handle(
        GetLocationByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for location by id query: {OrganizationId}",
                query.OrganizationId);
            return new Result<Location, GetLocationByIdAndOrganizationError>.Failure(
                GetLocationByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var location = await locationRepository.FindByIdAndOrganizationIdAsync(
                query.LocationId,
                query.OrganizationId,
                cancellationToken);
            if (location is null)
            {
                logger.LogWarning(
                    "Location not found for organization query: {OrganizationId} {LocationId}",
                    query.OrganizationId,
                    query.LocationId);
                return new Result<Location, GetLocationByIdAndOrganizationError>.Failure(
                    GetLocationByIdAndOrganizationError.LocationNotFound);
            }

            return new Result<Location, GetLocationByIdAndOrganizationError>.Success(location);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error querying location {LocationId} for organization {OrganizationId}",
                query.LocationId,
                query.OrganizationId);
            return new Result<Location, GetLocationByIdAndOrganizationError>.Failure(
                GetLocationByIdAndOrganizationError.UnexpectedError);
        }
    }
}
