using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a report detail query result.
/// </summary>
public static class ActionResultFromGetReportByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetReportByIdResult(
        Result<Report, GetReportByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<ReportsMessages> localizer) =>
        result switch
        {
            Result<Report, GetReportByIdAndOrganizationError>.Success success =>
                controller.Ok(ReportResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Report, GetReportByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetReportByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetReportByIdAndOrganizationError.ReportNotFound =>
                        controller.ProblemResponse(localizer, "ReportNotFound", StatusCodes.Status404NotFound),
                    GetReportByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingReports", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
