using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Monitoring.Application.Internal.CommandServices;

/// <summary>
///     Application service for sensor reading command operations.
/// </summary>
public class SensorReadingCommandService(
    ISensorReadingRepository sensorReadingRepository,
    IOrganizationRepository organizationRepository,
    IIotDeviceRepository iotDeviceRepository,
    IUnitOfWork unitOfWork,
    ILogger<SensorReadingCommandService> logger)
    : ISensorReadingCommandService
{
    /// <inheritdoc />
    public async Task<Result<SensorReading, CreateSensorReadingError>> Handle(
        CreateSensorReadingCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for sensor reading creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<SensorReading, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.OrganizationNotFound);
        }

        var iotDevice = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
            command.IotDeviceId,
            command.OrganizationId,
            cancellationToken);
        if (iotDevice is null)
        {
            logger.LogWarning("IoT device not found for sensor reading creation: {OrganizationId} {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<SensorReading, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.IotDeviceNotFound);
        }

        try
        {
            var sensorReading = new SensorReading(command);
            await sensorReadingRepository.AddAsync(sensorReading, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<SensorReading, CreateSensorReadingError>.Success(sensorReading);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating sensor reading for organization {OrganizationId} and IoT device {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<SensorReading, CreateSensorReadingError>.Failure(CreateSensorReadingError.UnexpectedError);
        }
    }
}
