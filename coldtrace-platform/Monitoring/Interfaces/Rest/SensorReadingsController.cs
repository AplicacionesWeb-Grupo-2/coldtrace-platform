using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using System.Net.Mime;
using ColdTrace.Platform.Monitoring.Domain.Model.Queries;
using ColdTrace.Platform.Monitoring.Application.CommandServices;
using ColdTrace.Platform.Monitoring.Application.QueryServices;
using ColdTrace.Platform.Monitoring.Interfaces.Rest.Resources;
using ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;
using ColdTrace.Platform.Monitoring.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest;

/// <summary>
///     Sensor readings controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/sensor-readings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Sensor Readings")]
public class SensorReadingsController(
    ISensorReadingCommandService sensorReadingCommandService,
    ISensorReadingQueryService sensorReadingQueryService,
    IStringLocalizer<MonitoringMessages> localizer,
    ILogger<SensorReadingsController> logger)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets sensor readings",
        Description = "Gets persisted telemetry readings for an organization with optional filters",
        OperationId = "GetSensorReadings")]
    [SwaggerResponse(200, "Sensor readings found", typeof(IEnumerable<SensorReadingResource>))]
    [SwaggerResponse(400, "The date range is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetSensorReadingsByOrganizationId(
        [FromRoute] int organizationId,
        [FromQuery] int? assetId,
        [FromQuery] int? iotDeviceId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken cancellationToken = default)
    {
        if (from is not null && to is not null && from > to) return this.ValidationProblemResponse(localizer, "InvalidRequest");

        var result = await sensorReadingQueryService.Handle(
            new GetSensorReadingsByOrganizationIdQuery(organizationId, assetId, iotDeviceId, from, to),
            cancellationToken);
        return ActionResultFromGetSensorReadingsByOrganizationResultAssembler
            .ToActionResultFromGetSensorReadingsByOrganizationResult(result, this, localizer);
    }

    [HttpGet("{sensorReadingId:int}")]
    [SwaggerOperation(
        Summary = "Gets sensor reading by id",
        Description = "Gets one sensor reading that belongs to the provided organization",
        OperationId = "GetSensorReadingById")]
    [SwaggerResponse(200, "Sensor reading found", typeof(SensorReadingResource))]
    [SwaggerResponse(404, "Organization or sensor reading not found", typeof(ProblemDetails))]
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

    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a sensor reading",
        Description = "Persists telemetry from an assigned online IoT device and evaluates it against asset settings",
        OperationId = "CreateSensorReading")]
    [SwaggerResponse(201, "The sensor reading was created", typeof(SensorReadingResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, asset, device or gateway not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateSensorReading(
        [FromRoute] int organizationId,
        [FromBody] CreateSensorReadingResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateSensorReadingCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId);
            var result = await sensorReadingCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateSensorReadingResultAssembler.ToActionResultFromCreateSensorReadingResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid sensor reading creation request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidSensorReadingRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating sensor reading for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingSensorReading", 500);
        }
    }

    [HttpPost("demo-generations")]
    [SwaggerOperation(
        Summary = "Generates demo sensor readings",
        Description = "Generates and persists realistic readings for eligible assigned online IoT devices",
        OperationId = "GenerateDemoSensorReadings")]
    [SwaggerResponse(201, "Sensor readings generated", typeof(IEnumerable<SensorReadingResource>))]
    [SwaggerResponse(400, "No eligible device or invalid generation request", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateDemoSensorReadings(
        [FromRoute] int organizationId,
        [FromBody] GenerateDemoSensorReadingsResource? resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = GenerateDemoSensorReadingsCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId);
            var result = await sensorReadingCommandService.Handle(command, cancellationToken);
            return ActionResultFromGenerateDemoSensorReadingsResultAssembler
                .ToActionResultFromGenerateDemoSensorReadingsResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid demo generation request for organization {OrganizationId}", organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidSensorReadingRequest");
        }
    }
}
