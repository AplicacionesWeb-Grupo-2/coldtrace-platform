using System.Net.Mime;
using ColdTrace.Platform.AiAssistance.Application.CommandServices;
using ColdTrace.Platform.AiAssistance.Application.QueryServices;
using ColdTrace.Platform.AiAssistance.Application.Internal.OutboundServices;
using ColdTrace.Platform.AiAssistance.Interfaces.Rest.Resources;
using ColdTrace.Platform.AiAssistance.Interfaces.Rest.Transform;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.AiAssistance.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest;

/// <summary>
///     REST controller for organization-scoped dashboard AI interpretation.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/dashboard")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("AI Assistance")]
public class DashboardAiInterpretationsController(
    IDashboardAiInterpretationCommandService dashboardAiInterpretationCommandService,
    IStringLocalizer<AiAssistanceMessages> localizer,
    ILogger<DashboardAiInterpretationsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Generates an advisory interpretation from persisted dashboard evidence.
    /// </summary>
    [HttpPost("ai-interpretation")]
    [SwaggerOperation(
        Summary = "Generates a dashboard AI interpretation",
        Description = "Loads persisted dashboard evidence and returns a structured advisory interpretation without mutating operational data",
        OperationId = "GenerateDashboardAiInterpretation")]
    [SwaggerResponse(200, "AI dashboard interpretation generated", typeof(DashboardAiInterpretationResource))]
    [SwaggerResponse(400, "Missing or invalid identifier or question", typeof(ProblemDetails))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Current subscription plan does not allow AI guidance", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Dashboard context could not be prepared", typeof(ProblemDetails))]
    [SwaggerResponse(502, "AI provider returned invalid structured output", typeof(ProblemDetails))]
    [SwaggerResponse(503, "AI provider is unavailable or disabled", typeof(ProblemDetails))]
    [SwaggerResponse(504, "AI provider timed out", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateDashboardAiInterpretation(
        [FromRoute] int organizationId,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
        [SwaggerRequestBody("Optional dashboard question and response language preference", Required = false)]
        GenerateDashboardAiInterpretationResource? resource = null,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguageHeader = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = GenerateDashboardAiInterpretationCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                acceptLanguageHeader);
            var result = await dashboardAiInterpretationCommandService.Handle(command, cancellationToken);
            return ActionResultFromGenerateDashboardAiInterpretationResultAssembler
                .ToActionResultFromGenerationResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid dashboard AI interpretation request for organization {OrganizationId}",
                organizationId);
            var message = localizer["InvalidDashboardAiInterpretationRequest"].Value;
            return Problem(title: message, detail: message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error generating dashboard AI interpretation for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorGeneratingDashboardAiInterpretation"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
