using System.Net.Mime;
using ColdTrace.Platform.Monitoring.Domain.Model.Queries;
using ColdTrace.Platform.Monitoring.Domain.Services;
using ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;
using ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST;

/// <summary>
///     Sensor readings controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Sensor Readings")]
public class SensorReadingsController(
    ISensorReadingCommandService sensorReadingCommandService,
    ISensorReadingQueryService sensorReadingQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<SensorReadingsController> logger)
    : ControllerBase
{
    [HttpGet("sensor-readings")]
    [SwaggerOperation(
        Summary = "Gets sensor readings by organization",
        Description = "Gets sensor readings that belong to the provided organization",
        OperationId = "GetSensorReadingsByOrganization")]
    [SwaggerResponse(200, "Sensor readings found", typeof(IEnumerable<SensorReadingResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetSensorReadingsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await sensorReadingQueryService.Handle(
            new GetSensorReadingsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetSensorReadingsByOrganizationResultAssembler
            .ToActionResultFromGetSensorReadingsByOrganizationResult(result, this, localizer);
    }

    [HttpGet("sensor-readings/{sensorReadingId:int}")]
    [SwaggerOperation(
        Summary = "Gets sensor reading by id",
        Description = "Gets one sensor reading that belongs to the provided organization",
        OperationId = "GetSensorReadingById")]
    [SwaggerResponse(200, "Sensor reading found", typeof(SensorReadingResource))]
    [SwaggerResponse(404, "Organization or sensor reading not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetSensorReadingById(
        [FromRoute] int organizationId,
        [FromRoute] int sensorReadingId,
        CancellationToken cancellationToken = default)
    {
        var result = await sensorReadingQueryService.Handle(
            new GetSensorReadingByIdAndOrganizationIdQuery(organizationId, sensorReadingId),
            cancellationToken);
        return ActionResultFromGetSensorReadingByIdResultAssembler.ToActionResultFromGetSensorReadingByIdResult(
            result,
            this,
            localizer);
    }

    [HttpGet("iot-devices/{iotDeviceId:int}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Gets sensor readings by IoT device",
        Description = "Gets sensor readings that belong to the provided organization IoT device",
        OperationId = "GetSensorReadingsByIotDevice")]
    [SwaggerResponse(200, "Sensor readings found", typeof(IEnumerable<SensorReadingResource>))]
    [SwaggerResponse(404, "Organization or IoT device not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetSensorReadingsByIotDeviceId(
        [FromRoute] int organizationId,
        [FromRoute] int iotDeviceId,
        CancellationToken cancellationToken = default)
    {
        var result = await sensorReadingQueryService.Handle(
            new GetSensorReadingsByIotDeviceAndOrganizationIdQuery(organizationId, iotDeviceId),
            cancellationToken);
        return ActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResultAssembler
            .ToActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResult(result, this, localizer);
    }

    [HttpPost("iot-devices/{iotDeviceId:int}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Creates a sensor reading",
        Description = "Creates a sensor reading for an organization IoT device",
        OperationId = "CreateSensorReading")]
    [SwaggerResponse(201, "The sensor reading was created", typeof(SensorReadingResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or IoT device not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateSensorReading(
        [FromRoute] int organizationId,
        [FromRoute] int iotDeviceId,
        [FromBody] CreateSensorReadingResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateSensorReadingCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                iotDeviceId);
            var result = await sensorReadingCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateSensorReadingResultAssembler.ToActionResultFromCreateSensorReadingResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid sensor reading creation request for organization {OrganizationId} and IoT device {IotDeviceId}",
                organizationId,
                iotDeviceId);
            return BadRequest(localizer["InvalidSensorReadingRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while creating sensor reading for organization {OrganizationId} and IoT device {IotDeviceId}",
                organizationId,
                iotDeviceId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorCreatingSensorReading"].Value,
                statusCode: 500);
        }
    }
}
