using System.Net.Mime;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Interfaces.REST.Resources;
using ColdTrace.Platform.Billing.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST;

/// <summary>
///     REST controller receiving signed Stripe billing webhooks.
/// </summary>
[ApiController]
[Route("api/v1/billing/stripe/webhooks")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Billing Stripe Webhooks")]
public class BillingStripeWebhooksController(
    IBillingWebhookCommandService billingWebhookCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<BillingStripeWebhooksController> logger)
    : ControllerBase
{
    private const string StripeSignatureHeader = "Stripe-Signature";

    /// <summary>
    ///     Processes a signed Stripe webhook.
    /// </summary>
    [AllowAnonymous] // Public transport endpoint authenticated by Stripe's signature validation.
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = "Process Stripe billing webhook",
        Description = "Verifies the Stripe signature and synchronizes local organization subscription state",
        OperationId = "ProcessStripeBillingWebhook")]
    [SwaggerResponse(200, "Webhook event processed or safely ignored", typeof(BillingWebhookProcessingResource))]
    [SwaggerResponse(400, "Missing signature, invalid signature, or invalid payload", typeof(ProblemDetails))]
    [SwaggerResponse(502, "Stripe webhook could not be processed", typeof(ProblemDetails))]
    [SwaggerResponse(503, "Stripe webhook signing secret is not configured", typeof(ProblemDetails))]
    public async Task<ActionResult> ProcessStripeWebhook(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("POST /api/v1/billing/stripe/webhooks");
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = Request.Headers[StripeSignatureHeader].FirstOrDefault();
        var result = await billingWebhookCommandService.Handle(
            new ProcessStripeWebhookCommand(payload, signatureHeader),
            cancellationToken);
        return ActionResultFromBillingWebhookCommandResultAssembler.ToActionResultFromResult(
            result,
            this,
            localizer);
    }
}
