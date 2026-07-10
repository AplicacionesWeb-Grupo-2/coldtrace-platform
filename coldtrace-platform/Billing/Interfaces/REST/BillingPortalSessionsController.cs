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
///     REST controller exposing billing customer portal session endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/billing")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Billing Customer Portal Sessions")]
public class BillingPortalSessionsController(
    IBillingPortalSessionCommandService billingPortalSessionCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<BillingPortalSessionsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Creates a provider-hosted customer portal session for billing management.
    /// </summary>
    [HttpPost("portal-sessions")]
    [SwaggerOperation(
        Summary = "Create billing customer portal session",
        Description = "Creates a provider-hosted Stripe Customer Portal session for an organization with Stripe billing state",
        OperationId = "CreateBillingPortalSession")]
    [SwaggerResponse(200, "Customer portal session created", typeof(BillingPortalSessionResource))]
    [SwaggerResponse(400, "The organization identifier is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or subscription not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Organization has no provider customer identifier", typeof(ProblemDetails))]
    [SwaggerResponse(502, "Stripe customer portal provider failed", typeof(ProblemDetails))]
    [SwaggerResponse(503, "Stripe customer portal provider is not configured", typeof(ProblemDetails))]
    public async Task<ActionResult> CreatePortalSession(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("POST /api/v1/organizations/{OrganizationId}/billing/portal-sessions", organizationId);
            var command = CreateBillingPortalSessionCommandFromResourceAssembler.ToCommandFromResource(organizationId);
            var result = await billingPortalSessionCommandService.Handle(command, cancellationToken);
            return ActionResultFromBillingPortalSessionCommandResultAssembler.ToActionResultFromResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid portal session request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "BillingPortalSessionInvalidRequest");
        }
    }

    /// <summary>
    ///     Creates a provider-hosted customer portal session through the ticket alias route.
    /// </summary>
    [HttpPost("customer-portal-sessions")]
    [SwaggerOperation(
        Summary = "Create billing customer portal session",
        Description = "Alias route for creating a provider-hosted Stripe Customer Portal session",
        OperationId = "CreateBillingCustomerPortalSession")]
    [SwaggerResponse(200, "Customer portal session created", typeof(BillingPortalSessionResource))]
    [SwaggerResponse(400, "The organization identifier is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or subscription not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Organization has no provider customer identifier", typeof(ProblemDetails))]
    [SwaggerResponse(502, "Stripe customer portal provider failed", typeof(ProblemDetails))]
    [SwaggerResponse(503, "Stripe customer portal provider is not configured", typeof(ProblemDetails))]
    public Task<ActionResult> CreateCustomerPortalSessionAlias(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default) =>
        CreatePortalSession(organizationId, cancellationToken);
}
