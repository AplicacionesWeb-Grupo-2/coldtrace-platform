using System.Net.Mime;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.MaintenanceManagement.Domain.Services;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST;

/// <summary>
///     REST controller exposing preventive maintenance schedule endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/maintenance-schedules")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Maintenance Schedules")]
public class MaintenanceSchedulesController(
    IMaintenanceScheduleCommandService maintenanceScheduleCommandService,
    IMaintenanceScheduleQueryService maintenanceScheduleQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<MaintenanceSchedulesController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all maintenance schedules for an organization.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets maintenance schedules by organization",
        Description = "Gets preventive maintenance schedules owned by the provided organization",
        OperationId = "GetMaintenanceSchedulesByOrganization")]
    [SwaggerResponse(200, "Maintenance schedules found", typeof(IEnumerable<MaintenanceScheduleResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetMaintenanceSchedulesByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await maintenanceScheduleQueryService.Handle(
            new GetMaintenanceSchedulesByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetMaintenanceSchedulesByOrganizationResultAssembler
            .ToActionResultFromGetMaintenanceSchedulesByOrganizationResult(result, this, localizer);
    }

    /// <summary>
    ///     Gets one maintenance schedule by identifier.
    /// </summary>
    [HttpGet("{maintenanceScheduleId:int}")]
    [SwaggerOperation(
        Summary = "Gets a maintenance schedule by id",
        Description = "Gets one preventive maintenance schedule owned by the provided organization",
        OperationId = "GetMaintenanceScheduleById")]
    [SwaggerResponse(200, "Maintenance schedule found", typeof(MaintenanceScheduleResource))]
    [SwaggerResponse(404, "Organization or maintenance schedule not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetMaintenanceScheduleById(
        [FromRoute] int organizationId,
        [FromRoute] int maintenanceScheduleId,
        CancellationToken cancellationToken = default)
    {
        var result = await maintenanceScheduleQueryService.Handle(
            new GetMaintenanceScheduleByIdAndOrganizationIdQuery(organizationId, maintenanceScheduleId),
            cancellationToken);
        return ActionResultFromGetMaintenanceScheduleByIdResultAssembler
            .ToActionResultFromGetMaintenanceScheduleByIdResult(result, this, localizer);
    }

    /// <summary>
    ///     Creates a preventive maintenance schedule for an organization asset.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a maintenance schedule",
        Description = "Creates a preventive maintenance schedule for an organization asset",
        OperationId = "CreateMaintenanceSchedule")]
    [SwaggerResponse(201, "Maintenance schedule created", typeof(MaintenanceScheduleResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(string))]
    [SwaggerResponse(409, "Asset already has an active maintenance schedule", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateMaintenanceSchedule(
        [FromRoute] int organizationId,
        [FromBody] CreateMaintenanceScheduleResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateMaintenanceScheduleCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId);
            var result = await maintenanceScheduleCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateMaintenanceScheduleResultAssembler
                .ToActionResultFromCreateMaintenanceScheduleResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid maintenance schedule creation request for organization {OrganizationId}",
                organizationId);
            return BadRequest(localizer["InvalidMaintenanceScheduleRequest"].Value);
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while creating maintenance schedule for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorCreatingMaintenanceSchedule"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Updates the lifecycle status of a maintenance schedule.
    /// </summary>
    [HttpPatch("{maintenanceScheduleId:int}")]
    [SwaggerOperation(
        Summary = "Updates maintenance schedule status",
        Description = "Updates the lifecycle status of a preventive maintenance schedule",
        OperationId = "UpdateMaintenanceScheduleStatus")]
    [SwaggerResponse(200, "Maintenance schedule status updated", typeof(MaintenanceScheduleResource))]
    [SwaggerResponse(400, "The request payload is invalid or status is not supported", typeof(string))]
    [SwaggerResponse(404, "Organization or maintenance schedule not found", typeof(string))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateMaintenanceScheduleStatus(
        [FromRoute] int organizationId,
        [FromRoute] int maintenanceScheduleId,
        [FromBody] UpdateMaintenanceScheduleStatusResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateMaintenanceScheduleStatusCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, maintenanceScheduleId);
            var result = await maintenanceScheduleCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateMaintenanceScheduleStatusResultAssembler
                .ToActionResultFromUpdateMaintenanceScheduleStatusResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid status update request for maintenance schedule {MaintenanceScheduleId} in organization {OrganizationId}",
                maintenanceScheduleId, organizationId);
            return BadRequest(localizer["InvalidMaintenanceScheduleRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while updating maintenance schedule {MaintenanceScheduleId} status",
                maintenanceScheduleId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorUpdatingMaintenanceSchedule"].Value,
                statusCode: 500);
        }
    }
}
