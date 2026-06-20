using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a report generation result.
/// </summary>
public static class ActionResultFromGenerateReportResultAssembler
{
    public static ActionResult ToActionResultFromGenerateReportResult(
        Result<Report, GenerateReportError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Report, GenerateReportError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    ReportResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Report, GenerateReportError>.Failure failure =>
                failure.Error switch
                {
                    GenerateReportError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GenerateReportError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGeneratingReport"].Value,
                            statusCode: 500),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
