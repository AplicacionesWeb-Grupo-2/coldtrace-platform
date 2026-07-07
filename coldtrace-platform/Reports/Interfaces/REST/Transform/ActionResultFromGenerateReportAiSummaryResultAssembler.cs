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
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GenerateReportAiSummaryError.ReportNotFound =>
                        controller.NotFound(localizer["ReportNotFound"].Value),
                    GenerateReportAiSummaryError.ReportContextUnavailable =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["ReportAiSummaryContextUnavailable"].Value,
                            statusCode: StatusCodes.Status500InternalServerError),
                    GenerateReportAiSummaryError.AiProviderDisabled =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderDisabled"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderNotConfigured =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderNotConfigured"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderUnavailable =>
                        controller.Problem(
                            title: localizer["AiProviderUnavailable"].Value,
                            detail: localizer["AiProviderRequestFailed"].Value,
                            statusCode: StatusCodes.Status503ServiceUnavailable),
                    GenerateReportAiSummaryError.AiProviderTimeout =>
                        controller.Problem(
                            title: localizer["AiProviderTimeout"].Value,
                            detail: localizer["AiProviderTimedOut"].Value,
                            statusCode: StatusCodes.Status504GatewayTimeout),
                    GenerateReportAiSummaryError.InvalidStructuredOutput =>
                        controller.Problem(
                            title: localizer["InvalidAiStructuredOutput"].Value,
                            detail: localizer["ReportAiSummaryInvalidStructuredOutput"].Value,
                            statusCode: StatusCodes.Status502BadGateway),
                    GenerateReportAiSummaryError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGeneratingReportAiSummary"].Value,
                            statusCode: StatusCodes.Status500InternalServerError),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: StatusCodes.Status500InternalServerError)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };
}
