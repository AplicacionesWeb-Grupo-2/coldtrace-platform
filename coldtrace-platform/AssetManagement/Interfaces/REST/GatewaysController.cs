using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST;

/// <summary>
///     Gateways controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/gateways")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Gateways")]
public class GatewaysController(
    IGatewayCommandService gatewayCommandService,
    IGatewayQueryService gatewayQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<GatewaysController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets gateways that belong to an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing gateway resources.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets gateways by organization",
        Description = "Gets edge gateways that belong to the provided organization",
        OperationId = "GetGatewaysByOrganization")]
    [SwaggerResponse(200, "Gateways found", typeof(IEnumerable<GatewayResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetGatewaysByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await gatewayQueryService.Handle(
            new GetGatewaysByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetGatewaysByOrganizationResultAssembler
            .ToActionResultFromGetGatewaysByOrganizationResult(
                result,
                this,
                localizer);
    }

    /// <summary>
    ///     Gets one gateway by id.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="gatewayId">Gateway identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing one gateway resource.</returns>
    [HttpGet("{gatewayId:int}")]
    [SwaggerOperation(
        Summary = "Gets gateway by id",
        Description = "Gets one edge gateway that belongs to the provided organization",
        OperationId = "GetGatewayById")]
    [SwaggerResponse(200, "Gateway found", typeof(GatewayResource))]
    [SwaggerResponse(404, "Organization or gateway not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetGatewayById(
        [FromRoute] int organizationId,
        [FromRoute] int gatewayId,
        CancellationToken cancellationToken = default)
    {
        var result = await gatewayQueryService.Handle(
            new GetGatewayByIdAndOrganizationIdQuery(organizationId, gatewayId),
            cancellationToken);
        return ActionResultFromGetGatewayByIdResultAssembler.ToActionResultFromGetGatewayByIdResult(
            result,
            this,
            localizer);
    }

    /// <summary>
    ///     Creates a gateway under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="resource">Gateway creation request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the created gateway resource.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a gateway",
        Description = "Creates an edge gateway for an organization location",
        OperationId = "CreateGateway")]
    [SwaggerResponse(201, "The gateway was created", typeof(GatewayResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or location not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Gateway UUID already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateGateway(
        [FromRoute] int organizationId,
        [FromBody] CreateGatewayResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateGatewayCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await gatewayCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateGatewayResultAssembler.ToActionResultFromCreateGatewayResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid gateway creation request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidGatewayRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while creating gateway for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingGateway", 500);
        }
    }

    /// <summary>
    ///     Updates a gateway under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="gatewayId">Gateway identifier.</param>
    /// <param name="resource">Gateway update request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the updated gateway resource.</returns>
    [HttpPut("{gatewayId:int}")]
    [SwaggerOperation(
        Summary = "Updates a gateway",
        Description = "Updates an edge gateway for an organization location",
        OperationId = "UpdateGateway")]
    [SwaggerResponse(200, "The gateway was updated", typeof(GatewayResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, location or gateway not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Gateway UUID already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateGateway(
        [FromRoute] int organizationId,
        [FromRoute] int gatewayId,
        [FromBody] UpdateGatewayResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateGatewayCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                gatewayId);
            var result = await gatewayCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateGatewayResultAssembler.ToActionResultFromUpdateGatewayResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid gateway update request for organization {OrganizationId} and gateway {GatewayId}",
                organizationId,
                gatewayId);
            return this.ValidationProblemResponse(localizer, "InvalidGatewayRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while updating gateway {GatewayId} for organization {OrganizationId}",
                gatewayId,
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorUpdatingGateway", 500);
        }
    }

    /// <summary>
    ///     Deletes a gateway under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="gatewayId">Gateway identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An empty response on success or an error response.</returns>
    [HttpDelete("{gatewayId:int}")]
    [SwaggerOperation(
        Summary = "Deletes a gateway",
        Description = "Deletes one edge gateway that belongs to the provided organization",
        OperationId = "DeleteGateway")]
    [SwaggerResponse(204, "Gateway deleted")]
    [SwaggerResponse(400, "The route identifiers are invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or gateway not found", typeof(string))]
    [SwaggerResponse(409, "Gateway cannot be deleted because related data exists", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> DeleteGateway(
        [FromRoute] int organizationId,
        [FromRoute] int gatewayId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await gatewayCommandService.Handle(
                new DeleteGatewayCommand(organizationId, gatewayId),
                cancellationToken);
            return ActionResultFromDeleteGatewayResultAssembler.ToActionResultFromDeleteGatewayResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid gateway deletion request for organization {OrganizationId} and gateway {GatewayId}",
                organizationId,
                gatewayId);
            return BadRequest(localizer["InvalidGatewayRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while deleting gateway {GatewayId} for organization {OrganizationId}",
                gatewayId,
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorDeletingGateway"].Value,
                statusCode: 500);
        }
    }
}
