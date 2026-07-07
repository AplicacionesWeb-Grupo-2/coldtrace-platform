using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles HTTP action results from billing webhook command results.
/// </summary>
public static class ActionResultFromBillingWebhookCommandResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<BillingWebhookProcessingResult, BillingWebhookError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<BillingWebhookProcessingResult, BillingWebhookError>.Success success =>
                controller.Ok(BillingWebhookProcessingResourceFromResultAssembler.ToResourceFromResult(success.Value)),
            Result<BillingWebhookProcessingResult, BillingWebhookError>.Failure failure =>
                ToFailureActionResult(failure.Error, controller, localizer),
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };

    private static ActionResult ToFailureActionResult(
        BillingWebhookError error,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
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
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };

    private static ActionResult Problem(
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer,
        string messageKey,
        int statusCode) =>
        controller.Problem(
            title: localizer[messageKey].Value,
            detail: localizer[messageKey].Value,
            statusCode: statusCode);
}
