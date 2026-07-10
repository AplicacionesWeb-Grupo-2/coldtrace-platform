using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Billing.Domain.Model.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Assembles HTTP action results from billing checkout session command results.
/// </summary>
public static class ActionResultFromBillingCheckoutSessionCommandResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<BillingCheckoutSession, BillingCheckoutSessionError> result,
        ControllerBase controller,
        IStringLocalizer<BillingMessages> localizer) =>
        result switch
        {
            Result<BillingCheckoutSession, BillingCheckoutSessionError>.Success success =>
                controller.Ok(BillingCheckoutSessionResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<BillingCheckoutSession, BillingCheckoutSessionError>.Failure failure =>
                ToFailureActionResult(failure.Error, controller, localizer),

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };

    private static ActionResult ToFailureActionResult(
        BillingCheckoutSessionError error,
        ControllerBase controller,
        IStringLocalizer<BillingMessages> localizer) =>
        error switch
        {
            BillingCheckoutSessionError.OrganizationNotFound =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionOrganizationNotFound", StatusCodes.Status404NotFound),
            BillingCheckoutSessionError.OrganizationSubscriptionNotFound =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionOrganizationSubscriptionNotFound", StatusCodes.Status404NotFound),
            BillingCheckoutSessionError.TargetPlanNotFound =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionTargetPlanNotFound", StatusCodes.Status404NotFound),
            BillingCheckoutSessionError.FreePlanCheckoutNotAllowed =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionFreePlanNotAllowed", StatusCodes.Status409Conflict),
            BillingCheckoutSessionError.PlanProviderPriceNotConfigured =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionPlanProviderPriceNotConfigured", StatusCodes.Status409Conflict),
            BillingCheckoutSessionError.ProviderNotConfigured =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionProviderNotConfigured", StatusCodes.Status503ServiceUnavailable),
            BillingCheckoutSessionError.ProviderUnavailable =>
                controller.ProblemResponse(localizer, "BillingCheckoutSessionProviderUnavailable", StatusCodes.Status502BadGateway),
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
