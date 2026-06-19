using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a report detail query result.
/// </summary>
public static class ActionResultFromGetReportByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetReportByIdResult(
        Result<Report, GetReportByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Report, GetReportByIdAndOrganizationError>.Success success =>
                controller.Ok(ReportResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Report, GetReportByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetReportByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetReportByIdAndOrganizationError.ReportNotFound =>
                        controller.NotFound(localizer["ReportNotFound"].Value),
                    GetReportByIdAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingReports"].Value,
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
