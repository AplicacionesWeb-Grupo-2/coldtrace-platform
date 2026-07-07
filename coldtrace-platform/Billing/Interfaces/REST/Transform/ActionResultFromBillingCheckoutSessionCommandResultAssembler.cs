using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles HTTP action results from billing checkout session command results.
/// </summary>
public static class ActionResultFromBillingCheckoutSessionCommandResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<BillingCheckoutSession, BillingCheckoutSessionError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<BillingCheckoutSession, BillingCheckoutSessionError>.Success success =>
                controller.Ok(BillingCheckoutSessionResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<BillingCheckoutSession, BillingCheckoutSessionError>.Failure failure =>
                ToFailureActionResult(failure.Error, controller, localizer),

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };

    private static ActionResult ToFailureActionResult(
        BillingCheckoutSessionError error,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        error switch
        {
            BillingCheckoutSessionError.OrganizationNotFound =>
                controller.NotFound(localizer["BillingCheckoutSessionOrganizationNotFound"].Value),
            BillingCheckoutSessionError.OrganizationSubscriptionNotFound =>
                controller.NotFound(localizer["BillingCheckoutSessionOrganizationSubscriptionNotFound"].Value),
            BillingCheckoutSessionError.TargetPlanNotFound =>
                controller.NotFound(localizer["BillingCheckoutSessionTargetPlanNotFound"].Value),
            BillingCheckoutSessionError.FreePlanCheckoutNotAllowed =>
                controller.Conflict(localizer["BillingCheckoutSessionFreePlanNotAllowed"].Value),
            BillingCheckoutSessionError.PlanProviderPriceNotConfigured =>
                controller.Conflict(localizer["BillingCheckoutSessionPlanProviderPriceNotConfigured"].Value),
            BillingCheckoutSessionError.ProviderNotConfigured =>
                controller.Problem(
                    title: localizer["BillingCheckoutSessionProviderNotConfigured"].Value,
                    detail: localizer["BillingCheckoutSessionProviderNotConfigured"].Value,
                    statusCode: StatusCodes.Status503ServiceUnavailable),
            BillingCheckoutSessionError.ProviderUnavailable =>
                controller.Problem(
                    title: localizer["BillingCheckoutSessionProviderUnavailable"].Value,
                    detail: localizer["BillingCheckoutSessionProviderUnavailable"].Value,
                    statusCode: StatusCodes.Status502BadGateway),
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
