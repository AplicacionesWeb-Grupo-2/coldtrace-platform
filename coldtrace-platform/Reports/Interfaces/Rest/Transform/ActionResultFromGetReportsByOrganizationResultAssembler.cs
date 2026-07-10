using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization reports query result.
/// </summary>
public static class ActionResultFromGetReportsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetReportsByOrganizationResult(
        Result<IEnumerable<Report>, GetReportsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<ReportsMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<Report>, GetReportsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(ReportResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Report>, GetReportsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetReportsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetReportsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingReports", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
