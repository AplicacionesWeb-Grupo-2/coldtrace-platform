using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST;

/// <summary>
///     IoT devices controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/iot-devices")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("IoT Devices")]
public class IotDevicesController(
    IIotDeviceCommandService iotDeviceCommandService,
    IIotDeviceQueryService iotDeviceQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<IotDevicesController> logger)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets IoT devices by organization",
        Description = "Gets IoT devices that belong to the provided organization",
        OperationId = "GetIotDevicesByOrganization")]
    [SwaggerResponse(200, "IoT devices found", typeof(IEnumerable<IotDeviceResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetIotDevicesByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await iotDeviceQueryService.Handle(
            new GetIotDevicesByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetIotDevicesByOrganizationResultAssembler
            .ToActionResultFromGetIotDevicesByOrganizationResult(result, this, localizer);
    }

    [HttpGet("{iotDeviceId:int}")]
    [SwaggerOperation(
        Summary = "Gets IoT device by id",
        Description = "Gets one IoT device that belongs to the provided organization",
        OperationId = "GetIotDeviceById")]
    [SwaggerResponse(200, "IoT device found", typeof(IotDeviceResource))]
    [SwaggerResponse(404, "Organization or IoT device not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetIotDeviceById(
        [FromRoute] int organizationId,
        [FromRoute] int iotDeviceId,
        CancellationToken cancellationToken = default)
    {
        var result = await iotDeviceQueryService.Handle(
            new GetIotDeviceByIdAndOrganizationIdQuery(organizationId, iotDeviceId),
            cancellationToken);
        return ActionResultFromGetIotDeviceByIdResultAssembler.ToActionResultFromGetIotDeviceByIdResult(
            result,
            this,
            localizer);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates an IoT device",
        Description = "Creates an IoT device for an organization gateway and optional asset",
        OperationId = "CreateIotDevice")]
    [SwaggerResponse(201, "The IoT device was created", typeof(IotDeviceResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, gateway or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "IoT device UUID already exists or asset is not compatible", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateIotDevice(
        [FromRoute] int organizationId,
        [FromBody] CreateIotDeviceResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateIotDeviceCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await iotDeviceCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateIotDeviceResultAssembler.ToActionResultFromCreateIotDeviceResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid IoT device creation request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidIotDeviceRequest");
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while creating IoT device for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingIotDevice", 500);
        }
    }

    [HttpPut("{iotDeviceId:int}")]
    [SwaggerOperation(
        Summary = "Updates an IoT device",
        Description = "Updates an IoT device for an organization gateway and optional asset",
        OperationId = "UpdateIotDevice")]
    [SwaggerResponse(200, "The IoT device was updated", typeof(IotDeviceResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, gateway, asset or IoT device not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "IoT device UUID already exists or asset is not compatible", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateIotDevice(
        [FromRoute] int organizationId,
        [FromRoute] int iotDeviceId,
        [FromBody] UpdateIotDeviceResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateIotDeviceCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                iotDeviceId);
            var result = await iotDeviceCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateIotDeviceResultAssembler.ToActionResultFromUpdateIotDeviceResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid IoT device update request for organization {OrganizationId} and device {IotDeviceId}",
                organizationId,
                iotDeviceId);
            return this.ValidationProblemResponse(localizer, "InvalidIotDeviceRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while updating IoT device {IotDeviceId} for organization {OrganizationId}",
                iotDeviceId,
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorUpdatingIotDevice", 500);
        }
    }

    [HttpDelete("{iotDeviceId:int}")]
    [SwaggerOperation(
        Summary = "Deletes an IoT device",
        Description = "Deletes one IoT device that belongs to the provided organization",
        OperationId = "DeleteIotDevice")]
    [SwaggerResponse(204, "The IoT device was deleted")]
    [SwaggerResponse(400, "The route identifiers are invalid", typeof(ProblemDetails))]
    [SwaggerResponse(404, "Organization or IoT device not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Related records prevent deleting the IoT device", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> DeleteIotDevice(
        [FromRoute] int organizationId,
        [FromRoute] int iotDeviceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteIotDeviceCommand(organizationId, iotDeviceId);
            var result = await iotDeviceCommandService.Handle(command, cancellationToken);
            return ActionResultFromDeleteIotDeviceResultAssembler.ToActionResultFromDeleteIotDeviceResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid IoT device deletion request for organization {OrganizationId} and device {IotDeviceId}",
                organizationId,
                iotDeviceId);
            return Problem(
                detail: localizer["InvalidIotDeviceRequest"].Value,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while deleting IoT device {IotDeviceId} for organization {OrganizationId}",
                iotDeviceId,
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorDeletingIotDevice"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
