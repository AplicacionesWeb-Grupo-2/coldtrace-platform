using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for IoT device query operations.
/// </summary>
public class IotDeviceQueryService(
    IIotDeviceRepository iotDeviceRepository,
    IOrganizationRepository organizationRepository,
    ILogger<IotDeviceQueryService> logger)
    : IIotDeviceQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>> Handle(
        GetIotDevicesByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for IoT device listing: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Failure(
                GetIotDevicesByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var devices = await iotDeviceRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Success(devices);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying IoT devices for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Failure(
                GetIotDevicesByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IotDevice, GetIotDeviceByIdAndOrganizationError>> Handle(
        GetIotDeviceByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for IoT device query: {OrganizationId}", query.OrganizationId);
            return new Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Failure(
                GetIotDeviceByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var device = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
                query.IotDeviceId,
                query.OrganizationId,
                cancellationToken);
            if (device is null)
            {
                logger.LogWarning("IoT device not found for organization query: {OrganizationId} {IotDeviceId}",
                    query.OrganizationId,
                    query.IotDeviceId);
                return new Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Failure(
                    GetIotDeviceByIdAndOrganizationError.IotDeviceNotFound);
            }

            return new Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Success(device);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying IoT device {IotDeviceId} for organization {OrganizationId}",
                query.IotDeviceId,
                query.OrganizationId);
            return new Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Failure(
                GetIotDeviceByIdAndOrganizationError.UnexpectedError);
        }
    }
}
