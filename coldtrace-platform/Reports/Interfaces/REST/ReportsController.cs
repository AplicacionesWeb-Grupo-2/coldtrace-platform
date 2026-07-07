using System.Net.Mime;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Reports.Domain.Model.Queries;
using ColdTrace.Platform.Reports.Domain.Services;
using ColdTrace.Platform.Reports.Interfaces.REST.Resources;
using ColdTrace.Platform.Reports.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Reports.Interfaces.REST;

/// <summary>
///     REST controller exposing organization-scoped report endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/reports")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Reports")]
public class ReportsController(
    IReportCommandService reportCommandService,
    IReportAiSummaryCommandService reportAiSummaryCommandService,
    IReportQueryService reportQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<ReportsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets reports for an organization.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets reports by organization",
        Description = "Gets generated reports owned by the provided organization",
        OperationId = "GetReportsByOrganization")]
    [SwaggerResponse(200, "Reports found", typeof(IEnumerable<ReportResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetReportsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await reportQueryService.Handle(
            new GetReportsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetReportsByOrganizationResultAssembler
            .ToActionResultFromGetReportsByOrganizationResult(result, this, localizer);
    }

    /// <summary>
    ///     Generates a report.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Generates a report",
        Description = "Generates an operational report from persisted backend data",
        OperationId = "GenerateReport")]
    [SwaggerResponse(201, "Report generated", typeof(ReportResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateReport(
        [FromRoute] int organizationId,
        [FromBody] GenerateReportResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = GenerateReportCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await reportCommandService.Handle(command, cancellationToken);
            return ActionResultFromGenerateReportResultAssembler
                .ToActionResultFromGenerateReportResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid report generation payload for organization {OrganizationId}",
                organizationId);
            return BadRequest(localizer["InvalidReportRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while generating report for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorGeneratingReport"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Gets one report by identifier.
    /// </summary>
    [HttpGet("{reportId:int}")]
    [SwaggerOperation(
        Summary = "Gets report by id",
        Description = "Gets one generated report owned by the provided organization",
        OperationId = "GetReportById")]
    [SwaggerResponse(200, "Report found", typeof(ReportResource))]
    [SwaggerResponse(404, "Organization or report not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetReportById(
        [FromRoute] int organizationId,
        [FromRoute] int reportId,
        CancellationToken cancellationToken = default)
    {
        var result = await reportQueryService.Handle(
            new GetReportByIdAndOrganizationIdQuery(organizationId, reportId),
            cancellationToken);
        return ActionResultFromGetReportByIdResultAssembler
            .ToActionResultFromGetReportByIdResult(result, this, localizer);
    }

    /// <summary>
    ///     Generates an advisory AI summary for one existing report.
    /// </summary>
    [HttpPost("{reportId:int}/ai-summary")]
    [SwaggerOperation(
        Summary = "Generates a report AI summary",
        Description = "Loads persisted report metrics and related evidence, then returns a structured advisory AI summary without mutating the source report",
        OperationId = "GenerateReportAiSummary")]
    [SwaggerResponse(200, "AI report summary generated", typeof(ReportAiSummaryResource))]
    [SwaggerResponse(400, "Missing or invalid identifier", typeof(string))]
    [SwaggerResponse(404, "Organization or report not found", typeof(string))]
    [SwaggerResponse(500, "Report context could not be prepared", typeof(ProblemDetails))]
    [SwaggerResponse(502, "AI provider returned invalid structured output", typeof(ProblemDetails))]
    [SwaggerResponse(503, "AI provider is unavailable or disabled", typeof(ProblemDetails))]
    [SwaggerResponse(504, "AI provider timed out", typeof(ProblemDetails))]
    public async Task<ActionResult> GenerateReportAiSummary(
        [FromRoute] int organizationId,
        [FromRoute] int reportId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await reportAiSummaryCommandService.Handle(
                new GenerateReportAiSummaryCommand(organizationId, reportId),
                cancellationToken);
            return ActionResultFromGenerateReportAiSummaryResultAssembler
                .ToActionResultFromGenerateReportAiSummaryResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid report AI summary route for organization {OrganizationId} and report {ReportId}",
                organizationId,
                reportId);
            return BadRequest(localizer["InvalidReportRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while generating AI report summary for organization {OrganizationId} and report {ReportId}",
                organizationId,
                reportId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorGeneratingReportAiSummary"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
