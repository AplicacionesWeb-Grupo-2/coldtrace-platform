using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a report generation result.
/// </summary>
public static class ActionResultFromGenerateReportResultAssembler
{
    public static ActionResult ToActionResultFromGenerateReportResult(
        Result<Report, GenerateReportError> result,
        ControllerBase controller,
        IStringLocalizer<ReportsMessages> localizer) =>
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
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GenerateReportError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGeneratingReport", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
