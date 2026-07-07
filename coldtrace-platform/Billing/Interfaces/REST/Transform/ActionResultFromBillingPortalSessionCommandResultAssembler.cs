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

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };

    private static ActionResult ToFailureActionResult(
        BillingPortalSessionError error,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        error switch
        {
            BillingPortalSessionError.OrganizationNotFound =>
                controller.NotFound(localizer["BillingPortalSessionOrganizationNotFound"].Value),
            BillingPortalSessionError.OrganizationSubscriptionNotFound =>
                controller.NotFound(localizer["BillingPortalSessionOrganizationSubscriptionNotFound"].Value),
            BillingPortalSessionError.ProviderCustomerNotFound =>
                controller.Conflict(localizer["BillingPortalSessionProviderCustomerNotFound"].Value),
            BillingPortalSessionError.ProviderNotConfigured =>
                controller.Problem(
                    title: localizer["BillingPortalSessionProviderNotConfigured"].Value,
                    detail: localizer["BillingPortalSessionProviderNotConfigured"].Value,
                    statusCode: StatusCodes.Status503ServiceUnavailable),
            BillingPortalSessionError.ProviderUnavailable =>
                controller.Problem(
                    title: localizer["BillingPortalSessionProviderUnavailable"].Value,
                    detail: localizer["BillingPortalSessionProviderUnavailable"].Value,
                    statusCode: StatusCodes.Status502BadGateway),
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
