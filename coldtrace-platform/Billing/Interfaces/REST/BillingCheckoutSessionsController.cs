using System.Net.Mime;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Interfaces.REST.Resources;
using ColdTrace.Platform.Billing.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST;

/// <summary>
///     REST controller exposing billing checkout session endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/billing/checkout-sessions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Billing Checkout Sessions")]
public class BillingCheckoutSessionsController(
    IBillingCheckoutSessionCommandService billingCheckoutSessionCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<BillingCheckoutSessionsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Creates a provider-hosted checkout session for paid plan upgrade.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create billing checkout session",
        Description = "Creates a provider-hosted Stripe Checkout session for a paid ColdTrace plan",
        OperationId = "CreateBillingCheckoutSession")]
    [SwaggerResponse(200, "Checkout session created", typeof(BillingCheckoutSessionResource))]
    [SwaggerResponse(400, "Missing or invalid request data", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, subscription, or target plan not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Plan is not eligible or configured for checkout", typeof(ProblemDetails))]
    [SwaggerResponse(502, "Stripe checkout provider failed", typeof(ProblemDetails))]
    [SwaggerResponse(503, "Stripe checkout provider is not configured", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateCheckoutSession(
        [FromRoute] int organizationId,
        [FromBody] CreateBillingCheckoutSessionResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateBillingCheckoutSessionCommandFromResourceAssembler.ToCommandFromResource(
                organizationId,
                resource);
            var result = await billingCheckoutSessionCommandService.Handle(command, cancellationToken);
            return ActionResultFromBillingCheckoutSessionCommandResultAssembler.ToActionResultFromResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid checkout session request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "BillingCheckoutSessionInvalidRequest");
        }
    }
}
