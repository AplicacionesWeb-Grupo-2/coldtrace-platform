using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Billing.Domain.Model.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Assembles HTTP action results from billing webhook command results.
/// </summary>
public static class ActionResultFromBillingWebhookCommandResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<BillingWebhookProcessingResult, BillingWebhookError> result,
        ControllerBase controller,
        IStringLocalizer<BillingMessages> localizer) =>
        result switch
        {
            Result<BillingWebhookProcessingResult, BillingWebhookError>.Success success =>
                controller.Ok(BillingWebhookProcessingResourceFromResultAssembler.ToResourceFromResult(success.Value)),
            Result<BillingWebhookProcessingResult, BillingWebhookError>.Failure failure =>
                ToFailureActionResult(failure.Error, controller, localizer),
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };

    private static ActionResult ToFailureActionResult(
        BillingWebhookError error,
        ControllerBase controller,
        IStringLocalizer<BillingMessages> localizer) =>
        error switch
        {
            BillingWebhookError.ProviderNotConfigured =>
                Problem(controller, localizer, "BillingWebhookProviderNotConfigured",
                    StatusCodes.Status503ServiceUnavailable),
            BillingWebhookError.MissingSignature =>
                Problem(controller, localizer, "BillingWebhookSignatureMissing",
                    StatusCodes.Status400BadRequest),
            BillingWebhookError.InvalidSignature =>
                Problem(controller, localizer, "BillingWebhookSignatureInvalid",
                    StatusCodes.Status400BadRequest),
            BillingWebhookError.InvalidPayload =>
                Problem(controller, localizer, "BillingWebhookPayloadInvalid",
                    StatusCodes.Status400BadRequest),
            BillingWebhookError.ProcessingFailed =>
                Problem(controller, localizer, "BillingWebhookProcessingFailed",
                    StatusCodes.Status502BadGateway),
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };

    private static ActionResult Problem(
        ControllerBase controller,
        IStringLocalizer<BillingMessages> localizer,
        string messageKey,
        int statusCode) =>
        controller.ProblemResponse(localizer, messageKey, statusCode);
}
