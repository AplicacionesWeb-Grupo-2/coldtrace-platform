using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles HTTP action results from billing portal session command results.
/// </summary>
public static class ActionResultFromBillingPortalSessionCommandResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<BillingPortalSession, BillingPortalSessionError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<BillingPortalSession, BillingPortalSessionError>.Success success =>
                controller.Ok(BillingPortalSessionResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<BillingPortalSession, BillingPortalSessionError>.Failure failure =>
                ToFailureActionResult(failure.Error, controller, localizer),

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };

    private static ActionResult ToFailureActionResult(
        BillingPortalSessionError error,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        error switch
        {
            BillingPortalSessionError.OrganizationNotFound =>
                controller.ProblemResponse(localizer, "BillingPortalSessionOrganizationNotFound", StatusCodes.Status404NotFound),
            BillingPortalSessionError.OrganizationSubscriptionNotFound =>
                controller.ProblemResponse(localizer, "BillingPortalSessionOrganizationSubscriptionNotFound", StatusCodes.Status404NotFound),
            BillingPortalSessionError.ProviderCustomerNotFound =>
                controller.ProblemResponse(localizer, "BillingPortalSessionProviderCustomerNotFound", StatusCodes.Status409Conflict),
            BillingPortalSessionError.ProviderNotConfigured =>
                controller.ProblemResponse(localizer, "BillingPortalSessionProviderNotConfigured", StatusCodes.Status503ServiceUnavailable),
            BillingPortalSessionError.ProviderUnavailable =>
                controller.ProblemResponse(localizer, "BillingPortalSessionProviderUnavailable", StatusCodes.Status502BadGateway),
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
