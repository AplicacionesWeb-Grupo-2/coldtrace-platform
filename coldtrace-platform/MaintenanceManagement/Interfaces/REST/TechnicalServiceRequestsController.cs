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
///     REST controller exposing corrective technical service request endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/technical-service-requests")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Technical Service Requests")]
public class TechnicalServiceRequestsController(
    ITechnicalServiceRequestCommandService technicalServiceRequestCommandService,
    ITechnicalServiceRequestQueryService technicalServiceRequestQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<TechnicalServiceRequestsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all technical service requests for an organization.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets technical service requests by organization",
        Description = "Gets corrective technical service requests owned by the provided organization",
        OperationId = "GetTechnicalServiceRequestsByOrganization")]
    [SwaggerResponse(200, "Technical service requests found", typeof(IEnumerable<TechnicalServiceRequestResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetTechnicalServiceRequestsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await technicalServiceRequestQueryService.Handle(
            new GetTechnicalServiceRequestsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetTechnicalServiceRequestsByOrganizationResultAssembler
            .ToActionResultFromGetTechnicalServiceRequestsByOrganizationResult(result, this, localizer);
    }

    /// <summary>
    ///     Gets one technical service request by identifier.
    /// </summary>
    [HttpGet("{technicalServiceRequestId:int}")]
    [SwaggerOperation(
        Summary = "Gets a technical service request by id",
        Description = "Gets one corrective technical service request owned by the provided organization",
        OperationId = "GetTechnicalServiceRequestById")]
    [SwaggerResponse(200, "Technical service request found", typeof(TechnicalServiceRequestResource))]
    [SwaggerResponse(404, "Organization or technical service request not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetTechnicalServiceRequestById(
        [FromRoute] int organizationId,
        [FromRoute] int technicalServiceRequestId,
        CancellationToken cancellationToken = default)
    {
        var result = await technicalServiceRequestQueryService.Handle(
            new GetTechnicalServiceRequestByIdAndOrganizationIdQuery(organizationId, technicalServiceRequestId),
            cancellationToken);
        return ActionResultFromGetTechnicalServiceRequestByIdResultAssembler
            .ToActionResultFromGetTechnicalServiceRequestByIdResult(result, this, localizer);
    }

    /// <summary>
    ///     Creates a corrective technical service request for an organization asset.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a technical service request",
        Description = "Opens a corrective technical service request for an organization asset",
        OperationId = "CreateTechnicalServiceRequest")]
    [SwaggerResponse(201, "Technical service request created", typeof(TechnicalServiceRequestResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateTechnicalServiceRequest(
        [FromRoute] int organizationId,
        [FromBody] CreateTechnicalServiceRequestResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateTechnicalServiceRequestCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId);
            var result = await technicalServiceRequestCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateTechnicalServiceRequestResultAssembler
                .ToActionResultFromCreateTechnicalServiceRequestResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid technical service request creation payload for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidTechnicalServiceRequest");
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while creating technical service request for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingTechnicalServiceRequest", 500);
        }
    }

    /// <summary>
    ///     Updates the lifecycle status of a technical service request.
    /// </summary>
    [HttpPatch("{technicalServiceRequestId:int}")]
    [SwaggerOperation(
        Summary = "Updates technical service request status",
        Description =
            "Updates the lifecycle status of a technical service request. Closure fields are required when transitioning to 'closed'.",
        OperationId = "UpdateTechnicalServiceRequestStatus")]
    [SwaggerResponse(200, "Technical service request status updated", typeof(TechnicalServiceRequestResource))]
    [SwaggerResponse(400, "The request payload is invalid or status is not supported", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or technical service request not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateTechnicalServiceRequestStatus(
        [FromRoute] int organizationId,
        [FromRoute] int technicalServiceRequestId,
        [FromBody] UpdateTechnicalServiceRequestStatusResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateTechnicalServiceRequestStatusCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, technicalServiceRequestId);
            var result = await technicalServiceRequestCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateTechnicalServiceRequestStatusResultAssembler
                .ToActionResultFromUpdateTechnicalServiceRequestStatusResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid status update request for technical service request {TechnicalServiceRequestId} in organization {OrganizationId}",
                technicalServiceRequestId, organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidTechnicalServiceRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while updating technical service request {TechnicalServiceRequestId} status",
                technicalServiceRequestId);
            return this.ProblemResponse(localizer, "UnexpectedErrorUpdatingTechnicalServiceRequest", 500);
        }
    }
}
