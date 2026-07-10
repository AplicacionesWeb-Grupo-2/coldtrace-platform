using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.CommandServices;
using ColdTrace.Platform.MaintenanceManagement.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.MaintenanceManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for maintenance schedule query operations.
/// </summary>
public class MaintenanceScheduleQueryService(
    IMaintenanceScheduleRepository maintenanceScheduleRepository,
    IIamContextFacade iamContextFacade,
    ILogger<MaintenanceScheduleQueryService> logger)
    : IMaintenanceScheduleQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>> Handle(
        GetMaintenanceSchedulesByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for maintenance schedules query: {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>.Failure(
                GetMaintenanceSchedulesByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var schedules = await maintenanceScheduleRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId, cancellationToken);
            return new Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>.Success(
                schedules);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying maintenance schedules for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>.Failure(
                GetMaintenanceSchedulesByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>> Handle(
        GetMaintenanceScheduleByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning(
                "Organization not found for maintenance schedule by id query: {OrganizationId}",
                query.OrganizationId);
            return new Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Failure(
                GetMaintenanceScheduleByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var schedule = await maintenanceScheduleRepository.FindByIdAndOrganizationIdAsync(
                query.MaintenanceScheduleId, query.OrganizationId, cancellationToken);
            if (schedule is null)
            {
                logger.LogWarning(
                    "Maintenance schedule not found: {OrganizationId} {MaintenanceScheduleId}",
                    query.OrganizationId, query.MaintenanceScheduleId);
                return new Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Failure(
                    GetMaintenanceScheduleByIdAndOrganizationError.MaintenanceScheduleNotFound);
            }

            return new Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Success(schedule);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error querying maintenance schedule {Id} for organization {OrganizationId}",
                query.MaintenanceScheduleId, query.OrganizationId);
            return new Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Failure(
                GetMaintenanceScheduleByIdAndOrganizationError.UnexpectedError);
        }
    }
}
