using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a report AI summary generation result.
/// </summary>
public static class ActionResultFromGenerateReportAiSummaryResultAssembler
{
    public static ActionResult ToActionResultFromGenerateReportAiSummaryResult(
        Result<ReportAiSummary, GenerateReportAiSummaryError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<ReportAiSummary, GenerateReportAiSummaryError>.Success success =>
                controller.Ok(ReportAiSummaryResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<ReportAiSummary, GenerateReportAiSummaryError>.Failure failure =>
                failure.Error switch
                {
                    GenerateReportAiSummaryError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GenerateReportAiSummaryError.ReportNotFound =>
                        controller.ProblemResponse(localizer, "ReportNotFound", StatusCodes.Status404NotFound),
                    GenerateReportAiSummaryError.ReportContextUnavailable =>
                        controller.ProblemResponse(localizer, "ReportAiSummaryContextUnavailable", StatusCodes.Status500InternalServerError),
                    GenerateReportAiSummaryError.AiProviderDisabled =>
                        controller.ProblemResponse(localizer, "AiProviderDisabled", StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderNotConfigured =>
                        controller.ProblemResponse(localizer, "AiProviderNotConfigured", StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderUnavailable =>
                        controller.ProblemResponse(localizer, "AiProviderRequestFailed", StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderTimeout =>
                        controller.ProblemResponse(localizer, "AiProviderTimedOut", StatusCodes.Status504GatewayTimeout),
                    GenerateReportAiSummaryError.InvalidStructuredOutput =>
                        controller.ProblemResponse(localizer, "ReportAiSummaryInvalidStructuredOutput", StatusCodes.Status502BadGateway),
                    GenerateReportAiSummaryError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGeneratingReportAiSummary", StatusCodes.Status500InternalServerError),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };
}
