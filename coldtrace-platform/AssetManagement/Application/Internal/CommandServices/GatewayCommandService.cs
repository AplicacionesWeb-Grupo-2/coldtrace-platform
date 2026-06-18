using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for gateway command operations.
/// </summary>
public class GatewayCommandService(
    IGatewayRepository gatewayRepository,
    ILocationRepository locationRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork,
    ILogger<GatewayCommandService> logger)
    : IGatewayCommandService
{
    /// <inheritdoc />
    public async Task<Result<Gateway, CreateGatewayError>> Handle(
        CreateGatewayCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for gateway creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for gateway creation: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.LocationNotFound);
        }

        if (await gatewayRepository.ExistsByOrganizationIdAndUuidAsync(
                command.OrganizationId,
                command.Uuid,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate gateway UUID rejected: {OrganizationId} {Uuid}",
                command.OrganizationId,
                command.Uuid);
            return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.DuplicateUuid);
        }

        try
        {
            var gateway = new Gateway(command);
            await gatewayRepository.AddAsync(gateway, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Gateway, CreateGatewayError>.Success(gateway);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateGatewayUuidError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation creating gateway {Uuid} for organization {OrganizationId}",
                    command.Uuid,
                    command.OrganizationId);
                return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.DuplicateUuid);
            }

            logger.LogError(
                ex,
                "Database update failed creating gateway {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error creating gateway {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<Gateway, CreateGatewayError>.Failure(CreateGatewayError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Gateway, UpdateGatewayError>> Handle(
        UpdateGatewayCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for gateway update: {OrganizationId}",
                command.OrganizationId);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for gateway update: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.LocationNotFound);
        }

        var gateway = await gatewayRepository.FindByIdAndOrganizationIdAsync(
            command.GatewayId,
            command.OrganizationId,
            cancellationToken);
        if (gateway is null)
        {
            logger.LogWarning(
                "Gateway not found for update: {OrganizationId} {GatewayId}",
                command.OrganizationId,
                command.GatewayId);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.GatewayNotFound);
        }

        if (await gatewayRepository.ExistsByOrganizationIdAndUuidAndIdNotAsync(
                command.OrganizationId,
                command.Uuid,
                command.GatewayId,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate gateway UUID rejected for update: {OrganizationId} {GatewayId} {Uuid}",
                command.OrganizationId,
                command.GatewayId,
                command.Uuid);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.DuplicateUuid);
        }

        try
        {
            gateway.Update(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Gateway, UpdateGatewayError>.Success(gateway);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateGatewayUuidError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation updating gateway {GatewayId} for organization {OrganizationId}",
                    command.GatewayId,
                    command.OrganizationId);
                return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.DuplicateUuid);
            }

            logger.LogError(
                ex,
                "Database update failed updating gateway {GatewayId} for organization {OrganizationId}",
                command.GatewayId,
                command.OrganizationId);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error updating gateway {GatewayId} for organization {OrganizationId}",
                command.GatewayId,
                command.OrganizationId);
            return new Result<Gateway, UpdateGatewayError>.Failure(UpdateGatewayError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateGatewayUuidError(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062 &&
                current.Message.Contains("i_x_gateways_organization_id_uuid", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
