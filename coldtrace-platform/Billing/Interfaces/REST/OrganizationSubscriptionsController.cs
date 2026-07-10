using System.Net.Mime;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Interfaces.REST.Resources;
using ColdTrace.Platform.Billing.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST;

/// <summary>
///     REST controller exposing organization subscription endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/subscription")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Organization Subscriptions")]
public class OrganizationSubscriptionsController(
    IOrganizationSubscriptionQueryService organizationSubscriptionQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<OrganizationSubscriptionsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets the current organization subscription.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get organization subscription",
        Description = "Gets an organization's active plan, billing state, usage and entitlements",
        OperationId = "GetOrganizationSubscription")]
    [SwaggerResponse(200, "Organization subscription found", typeof(OrganizationSubscriptionResource))]
    [SwaggerResponse(400, "The organization identifier is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or subscription data not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetOrganizationSubscription(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await organizationSubscriptionQueryService.Handle(
                new GetOrganizationSubscriptionByOrganizationIdQuery(organizationId),
                cancellationToken);
            return ActionResultFromOrganizationSubscriptionQueryResultAssembler.ToActionResultFromResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid organization subscription query for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidOrganizationRequest");
        }
    }
}
