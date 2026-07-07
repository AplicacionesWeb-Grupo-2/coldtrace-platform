using System.Net.Mime;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;
using ColdTrace.Platform.Alerts.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.REST;

/// <summary>
///     REST controller exposing organization-scoped incident endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/incidents")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Incidents")]
public class IncidentsController(
    IIncidentCommandService incidentCommandService,
    IAiResolutionPlanCommandService aiResolutionPlanCommandService,
    IIncidentQueryService incidentQueryService,
    INotificationQueryService notificationQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<IncidentsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets incidents for an organization.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets incidents by organization",
        Description = "Gets incidents owned by the provided organization",
        OperationId = "GetIncidentsByOrganization")]
    [SwaggerResponse(200, "Incidents found", typeof(IEnumerable<IncidentResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetIncidentsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await incidentQueryService.Handle(
            new GetIncidentsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetIncidentsByOrganizationResultAssembler
            .ToActionResultFromGetIncidentsByOrganizationResult(result, this, localizer);
    }

    /// <summary>
    ///     Gets one incident by identifier.
    /// </summary>
    [HttpGet("{incidentId:int}")]
    [SwaggerOperation(
        Summary = "Gets incident by id",
        Description = "Gets one incident owned by the provided organization",
        OperationId = "GetIncidentById")]
    [SwaggerResponse(200, "Incident found", typeof(IncidentResource))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetIncidentById(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        CancellationToken cancellationToken = default)
    {
        var result = await incidentQueryService.Handle(
            new GetIncidentByIdAndOrganizationIdQuery(organizationId, incidentId),
            cancellationToken);
        return ActionResultFromGetIncidentByIdResultAssembler
            .ToActionResultFromGetIncidentByIdResult(result, this, localizer);
    }

    /// <summary>
    ///     Generates an AI resolution plan for an active incident.
    /// </summary>
    [HttpPost("{incidentId:int}/ai-resolution-plans")]
    [SwaggerOperation(
        Summary = "Generates an AI incident resolution plan",
        Description = "Generates and persists a pending AI resolution plan from real incident context",
        OperationId = "GenerateAiResolutionPlan")]
    [SwaggerResponse(201, "AI resolution plan generated", typeof(AiResolutionPlanResource))]
    [SwaggerResponse(400, "The route values are invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(409, "Incident cannot receive AI resolution plans", typeof(string))]
    [SwaggerResponse(502, "AI provider returned invalid structured output", typeof(ProblemDetails))]
    [SwaggerResponse(503, "AI provider is disabled, unavailable, or not configured", typeof(ProblemDetails))]
    [SwaggerResponse(504, "AI provider timed out", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateAiResolutionPlan(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = GenerateAiResolutionPlanCommandFromRouteAssembler
                .ToCommandFromRoute(organizationId, incidentId);
            var result = await aiResolutionPlanCommandService.Handle(command, cancellationToken);
            return ActionResultFromGenerateAiResolutionPlanResultAssembler
                .ToActionResultFromGenerateAiResolutionPlanResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid AI resolution plan route values for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while generating AI resolution plan for incident {IncidentId}",
                incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorGeneratingAiResolutionPlan"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Approves an AI resolution plan and resolves its incident.
    /// </summary>
    [HttpPost("{incidentId:int}/ai-resolution-plans/{planId:int}/approvals")]
    [SwaggerOperation(
        Summary = "Approves an incident AI resolution plan",
        Description = "Approves a pending AI plan, stores final operator notes and resolves the incident through backend lifecycle rules",
        OperationId = "ApproveAiResolutionPlan")]
    [SwaggerResponse(200, "AI resolution plan approved and incident resolved", typeof(AiResolutionPlanResource))]
    [SwaggerResponse(400, "Missing or invalid approval request", typeof(string))]
    [SwaggerResponse(404, "Organization, incident, or AI plan not found", typeof(string))]
    [SwaggerResponse(409, "Plan already decided or incident already resolved", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ApproveAiResolutionPlan(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromRoute] int planId,
        [FromBody] ApproveAiResolutionPlanResource? resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (resource is null) throw new ArgumentException("Approval request body is required.");

            var command = ApproveAiResolutionPlanCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId, planId);
            var result = await aiResolutionPlanCommandService.Handle(command, cancellationToken);
            return ActionResultFromApproveAiResolutionPlanResultAssembler
                .ToActionResultFromApproveAiResolutionPlanResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid AI resolution plan approval request for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidAiResolutionPlanApprovalRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while approving AI resolution plan {PlanId} for incident {IncidentId}",
                planId,
                incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorApprovingAiResolutionPlan"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Rejects an AI resolution plan without changing incident status.
    /// </summary>
    [HttpPost("{incidentId:int}/ai-resolution-plans/{planId:int}/rejections")]
    [SwaggerOperation(
        Summary = "Rejects an incident AI resolution plan",
        Description = "Rejects a pending AI plan, stores operator audit metadata and leaves the incident lifecycle unchanged",
        OperationId = "RejectAiResolutionPlan")]
    [SwaggerResponse(200, "AI resolution plan rejected", typeof(AiResolutionPlanResource))]
    [SwaggerResponse(400, "Missing or invalid rejection request", typeof(string))]
    [SwaggerResponse(404, "Organization, incident, or AI plan not found", typeof(string))]
    [SwaggerResponse(409, "Plan already approved or rejected", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RejectAiResolutionPlan(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromRoute] int planId,
        [FromBody] RejectAiResolutionPlanResource? resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (resource is null) throw new ArgumentException("Rejection request body is required.");

            var command = RejectAiResolutionPlanCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId, planId);
            var result = await aiResolutionPlanCommandService.Handle(command, cancellationToken);
            return ActionResultFromRejectAiResolutionPlanResultAssembler
                .ToActionResultFromRejectAiResolutionPlanResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid AI resolution plan rejection request for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidAiResolutionPlanRejectionRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while rejecting AI resolution plan {PlanId} for incident {IncidentId}",
                planId,
                incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorRejectingAiResolutionPlan"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Creates an incident.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates an incident",
        Description = "Registers a backend-owned incident for an organization",
        OperationId = "CreateIncident")]
    [SwaggerResponse(201, "Incident created", typeof(IncidentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateIncident(
        [FromRoute] int organizationId,
        [FromBody] CreateIncidentResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateIncidentCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await incidentCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateIncidentResultAssembler
                .ToActionResultFromCreateIncidentResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid incident creation payload for organization {OrganizationId}", organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating incident for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorCreatingIncident"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Acknowledges an open incident.
    /// </summary>
    [HttpPost("{incidentId:int}/acknowledgements")]
    [SwaggerOperation(
        Summary = "Acknowledges an incident",
        Description = "Moves an open incident to the acknowledged lifecycle state",
        OperationId = "AcknowledgeIncident")]
    [SwaggerResponse(200, "Incident acknowledged", typeof(IncidentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> AcknowledgeIncident(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromBody] AcknowledgeIncidentResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = AcknowledgeIncidentCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId);
            var result = await incidentCommandService.Handle(command, cancellationToken);
            return ActionResultFromAcknowledgeIncidentResultAssembler
                .ToActionResultFromAcknowledgeIncidentResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid acknowledgement payload for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while acknowledging incident {IncidentId}", incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorAcknowledgingIncident"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Escalates an active incident.
    /// </summary>
    [HttpPatch("{incidentId:int}/escalation")]
    [SwaggerOperation(
        Summary = "Escalates an incident",
        Description = "Registers escalation fields for an open or acknowledged incident",
        OperationId = "EscalateIncident")]
    [SwaggerResponse(200, "Incident escalated", typeof(IncidentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> EscalateIncident(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromBody] EscalateIncidentResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = EscalateIncidentCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId);
            var result = await incidentCommandService.Handle(command, cancellationToken);
            return ActionResultFromEscalateIncidentResultAssembler
                .ToActionResultFromEscalateIncidentResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid escalation payload for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while escalating incident {IncidentId}", incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorEscalatingIncident"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Registers corrective action for an active incident.
    /// </summary>
    [HttpPatch("{incidentId:int}/corrective-action")]
    [SwaggerOperation(
        Summary = "Registers incident corrective action",
        Description = "Registers corrective action fields for an open or acknowledged incident",
        OperationId = "RegisterIncidentCorrectiveAction")]
    [SwaggerResponse(200, "Corrective action registered", typeof(IncidentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> RegisterIncidentCorrectiveAction(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromBody] RegisterIncidentCorrectiveActionResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = RegisterIncidentCorrectiveActionCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId);
            var result = await incidentCommandService.Handle(command, cancellationToken);
            return ActionResultFromRegisterIncidentCorrectiveActionResultAssembler
                .ToActionResultFromRegisterIncidentCorrectiveActionResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid corrective action payload for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while registering corrective action for incident {IncidentId}",
                incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorRegisteringIncidentCorrectiveAction"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Resolves an active incident.
    /// </summary>
    [HttpPost("{incidentId:int}/resolutions")]
    [SwaggerOperation(
        Summary = "Resolves an incident",
        Description = "Moves an open or acknowledged incident to the resolved lifecycle state",
        OperationId = "ResolveIncident")]
    [SwaggerResponse(200, "Incident resolved", typeof(IncidentResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(409, "Lifecycle transition is not allowed", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> ResolveIncident(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        [FromBody] ResolveIncidentResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = ResolveIncidentCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, incidentId);
            var result = await incidentCommandService.Handle(command, cancellationToken);
            return ActionResultFromResolveIncidentResultAssembler
                .ToActionResultFromResolveIncidentResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid resolution payload for incident {IncidentId} in organization {OrganizationId}",
                incidentId,
                organizationId);
            return BadRequest(localizer["InvalidIncidentRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while resolving incident {IncidentId}", incidentId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorResolvingIncident"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Gets notifications derived from one incident.
    /// </summary>
    [HttpGet("{incidentId:int}/notifications")]
    [SwaggerOperation(
        Summary = "Gets incident notifications",
        Description = "Gets notification read models derived from one incident",
        OperationId = "GetIncidentNotifications")]
    [SwaggerResponse(200, "Notifications found", typeof(IEnumerable<NotificationResource>))]
    [SwaggerResponse(404, "Organization or incident not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetNotificationsByIncidentId(
        [FromRoute] int organizationId,
        [FromRoute] int incidentId,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationQueryService.Handle(
            new GetNotificationsByIncidentIdAndOrganizationIdQuery(organizationId, incidentId),
            cancellationToken);
        return ActionResultFromGetNotificationsByIncidentResultAssembler
            .ToActionResultFromGetNotificationsByIncidentResult(result, this, localizer);
    }
}
