using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for location command operations.
/// </summary>
public class LocationCommandService(
    ILocationRepository locationRepository,
    IOrganizationRepository organizationRepository,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<LocationCommandService> logger)
    : ILocationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Location, CreateLocationError>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for location creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<Location, CreateLocationError>.Failure(CreateLocationError.OrganizationNotFound);
        }

        if (await locationRepository.ExistsByOrganizationIdAndNameAsync(
                command.OrganizationId,
                command.Name,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate location name rejected: {OrganizationId} {Name}",
                command.OrganizationId,
                command.Name);
            return new Result<Location, CreateLocationError>.Failure(CreateLocationError.DuplicateName);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementLocations,
            "LocationPlanLimitExceeded",
            cancellationToken);

        try
        {
            var location = new Location(command);
            await locationRepository.AddAsync(location, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Location, CreateLocationError>.Success(location);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateLocationNameError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation creating location {Name} for organization {OrganizationId}",
                    command.Name,
                    command.OrganizationId);
                return new Result<Location, CreateLocationError>.Failure(CreateLocationError.DuplicateName);
            }

            logger.LogError(
                ex,
                "Database update failed creating location {Name} for organization {OrganizationId}",
                command.Name,
                command.OrganizationId);
            return new Result<Location, CreateLocationError>.Failure(CreateLocationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error creating location {Name} for organization {OrganizationId}",
                command.Name,
                command.OrganizationId);
            return new Result<Location, CreateLocationError>.Failure(CreateLocationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Location, UpdateLocationError>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for location update: {OrganizationId}",
                command.OrganizationId);
            return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for update: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.LocationNotFound);
        }

        if (await locationRepository.ExistsByOrganizationIdAndNameAndIdNotAsync(
                command.OrganizationId,
                command.Name,
                command.LocationId,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate location name rejected for update: {OrganizationId} {LocationId} {Name}",
                command.OrganizationId,
                command.LocationId,
                command.Name);
            return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.DuplicateName);
        }

        try
        {
            location.Update(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Location, UpdateLocationError>.Success(location);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateLocationNameError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation updating location {LocationId} for organization {OrganizationId}",
                    command.LocationId,
                    command.OrganizationId);
                return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.DuplicateName);
            }

            logger.LogError(
                ex,
                "Database update failed updating location {LocationId} for organization {OrganizationId}",
                command.LocationId,
                command.OrganizationId);
            return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error updating location {LocationId} for organization {OrganizationId}",
                command.LocationId,
                command.OrganizationId);
            return new Result<Location, UpdateLocationError>.Failure(UpdateLocationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteLocationCommand, DeleteLocationError>> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for location deletion: {OrganizationId}",
                command.OrganizationId);
            return new Result<DeleteLocationCommand, DeleteLocationError>.Failure(
                DeleteLocationError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for deletion: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<DeleteLocationCommand, DeleteLocationError>.Failure(
                DeleteLocationError.LocationNotFound);
        }

        try
        {
            locationRepository.Remove(location);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation(
                "Location deleted: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<DeleteLocationCommand, DeleteLocationError>.Success(command);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(
                ex,
                "Location deletion blocked by a database relationship: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<DeleteLocationCommand, DeleteLocationError>.Failure(
                DeleteLocationError.DeleteBlocked);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error deleting location {LocationId} for organization {OrganizationId}",
                command.LocationId,
                command.OrganizationId);
            return new Result<DeleteLocationCommand, DeleteLocationError>.Failure(
                DeleteLocationError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateLocationNameError(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062 &&
                current.Message.Contains("i_x_locations_organization_id_name", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
