using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Queries;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Monitoring.Application.Internal.QueryServices;

/// <summary>
///     Application service for sensor reading query operations.
/// </summary>
public class SensorReadingQueryService(
    ISensorReadingRepository sensorReadingRepository,
    IOrganizationRepository organizationRepository,
    IIotDeviceRepository iotDeviceRepository,
    ILogger<SensorReadingQueryService> logger)
    : ISensorReadingQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>> Handle(
        GetSensorReadingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for sensor readings listing: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>.Failure(
                GetSensorReadingsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var readings = await sensorReadingRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>.Success(readings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying sensor readings for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>.Failure(
                GetSensorReadingsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>> Handle(
        GetSensorReadingsByIotDeviceAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for device sensor readings listing: {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Failure(
                GetSensorReadingsByIotDeviceAndOrganizationError.OrganizationNotFound);
        }

        var iotDevice = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
            query.IotDeviceId,
            query.OrganizationId,
            cancellationToken);
        if (iotDevice is null)
        {
            logger.LogWarning("IoT device not found for sensor readings listing: {OrganizationId} {IotDeviceId}",
                query.OrganizationId,
                query.IotDeviceId);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Failure(
                GetSensorReadingsByIotDeviceAndOrganizationError.IotDeviceNotFound);
        }

        try
        {
            var readings = await sensorReadingRepository.FindAllByOrganizationIdAndIotDeviceIdAsync(
                query.OrganizationId,
                query.IotDeviceId,
                cancellationToken);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Success(
                readings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying sensor readings for organization {OrganizationId} and IoT device {IotDeviceId}",
                query.OrganizationId,
                query.IotDeviceId);
            return new Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Failure(
                GetSensorReadingsByIotDeviceAndOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<SensorReading, GetSensorReadingByIdAndOrganizationError>> Handle(
        GetSensorReadingByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for sensor reading query: {OrganizationId}", query.OrganizationId);
            return new Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Failure(
                GetSensorReadingByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var reading = await sensorReadingRepository.FindByIdAndOrganizationIdAsync(
                query.SensorReadingId,
                query.OrganizationId,
                cancellationToken);
            if (reading is null)
            {
                logger.LogWarning("Sensor reading not found for organization query: {OrganizationId} {SensorReadingId}",
                    query.OrganizationId,
                    query.SensorReadingId);
                return new Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Failure(
                    GetSensorReadingByIdAndOrganizationError.SensorReadingNotFound);
            }

            return new Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Success(reading);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying sensor reading {SensorReadingId} for organization {OrganizationId}",
                query.SensorReadingId,
                query.OrganizationId);
            return new Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Failure(
                GetSensorReadingByIdAndOrganizationError.UnexpectedError);
        }
    }
}
