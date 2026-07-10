using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization reports query result.
/// </summary>
public static class ActionResultFromGetReportsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetReportsByOrganizationResult(
        Result<IEnumerable<Report>, GetReportsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
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
