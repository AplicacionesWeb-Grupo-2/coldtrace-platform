using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles HTTP action results from organization subscription query results.
/// </summary>
public static class ActionResultFromOrganizationSubscriptionQueryResultAssembler
{
    public static ActionResult ToActionResultFromResult(
        Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Success success =>
                controller.Ok(OrganizationSubscriptionResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Failure failure =>
                failure.Error switch
                {
                    GetOrganizationSubscriptionError.OrganizationNotFound =>
                        controller.NotFound(localizer["BillingOrganizationSubscriptionOrganizationNotFound"].Value),
                    GetOrganizationSubscriptionError.OrganizationSubscriptionNotFound =>
                        controller.NotFound(localizer["BillingOrganizationSubscriptionNotFound"].Value),
                    GetOrganizationSubscriptionError.SubscriptionPlanNotFound =>
                        controller.NotFound(localizer["BillingOrganizationSubscriptionPlanNotFound"].Value),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
