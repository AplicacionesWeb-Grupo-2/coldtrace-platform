using System.Net.Mime;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Application.CommandServices;
using ColdTrace.Platform.Billing.Application.QueryServices;
using ColdTrace.Platform.Billing.Interfaces.Rest.Resources;
using ColdTrace.Platform.Billing.Interfaces.Rest.Transform;
using ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.Rest;

/// <summary>
///     REST controller exposing public subscription plan catalog endpoints.
/// </summary>
[ApiController]
[Route("api/v1/subscription-plans")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscription Plans")]
public class SubscriptionPlansController(ISubscriptionPlanQueryService subscriptionPlanQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets active subscription plans.
    /// </summary>
    [AllowAnonymous] // Public read-only catalog consumed by the landing page.
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets subscription plans",
        Description = "Gets active ColdTrace subscription plans with pricing, limits, and feature flags",
        OperationId = "GetSubscriptionPlans")]
    [SwaggerResponse(200, "Subscription plans found", typeof(IEnumerable<SubscriptionPlanResource>))]
    public async Task<ActionResult> GetSubscriptionPlans(CancellationToken cancellationToken = default)
    {
        var plans = await subscriptionPlanQueryService.Handle(
            new GetActiveSubscriptionPlansQuery(),
            cancellationToken);

        return ActionResultFromSubscriptionPlanQueryResultAssembler.ToActionResultFromList(plans, this);
    }
}
