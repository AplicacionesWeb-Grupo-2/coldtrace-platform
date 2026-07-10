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
                        controller.ProblemResponse(localizer, "BillingOrganizationSubscriptionOrganizationNotFound", StatusCodes.Status404NotFound),
                    GetOrganizationSubscriptionError.OrganizationSubscriptionNotFound =>
                        controller.ProblemResponse(localizer, "BillingOrganizationSubscriptionNotFound", StatusCodes.Status404NotFound),
                    GetOrganizationSubscriptionError.SubscriptionPlanNotFound =>
                        controller.ProblemResponse(localizer, "BillingOrganizationSubscriptionPlanNotFound", StatusCodes.Status404NotFound),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
