using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Infrastructure.Configuration;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ColdTrace.Platform.Billing.Infrastructure.Stripe;

/// <summary>
///     Stripe-backed checkout session provider adapter.
/// </summary>
public class StripeCheckoutSessionProviderService(
    IOptions<BillingOptions> options,
    ILogger<StripeCheckoutSessionProviderService> logger)
    : ICheckoutSessionProviderService
{
    private const string MetadataOrganizationId = "organizationId";
    private const string MetadataTargetPlanCode = "targetPlanCode";

    public async Task<Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>>
        CreateSubscriptionCheckoutSessionAsync(
            CheckoutSessionProviderRequest request,
            CancellationToken cancellationToken = default)
    {
        var stripeOptions = options.Value.Stripe;
        if (!stripeOptions.HasCheckoutConfiguration())
        {
            logger.LogWarning("Stripe checkout configuration is incomplete");
            return Failure(CheckoutSessionProviderFailure.NotConfigured);
        }

        try
        {
            var service = new SessionService();
            var session = await service.CreateAsync(
                ToStripeSessionCreateOptions(request, stripeOptions),
                new RequestOptions { ApiKey = stripeOptions.SecretKey },
                cancellationToken);

            if (string.IsNullOrWhiteSpace(session.Url))
            {
                logger.LogWarning("Stripe checkout session did not include a redirect URL: {SessionId}", session.Id);
                return Failure(CheckoutSessionProviderFailure.Unavailable);
            }

            return new Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>.Success(
                new CheckoutSessionProviderResult(BillingProviders.Stripe, session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex,
                "Stripe checkout session creation failed: organizationId={OrganizationId}, status={Status}, code={Code}",
                request.OrganizationId,
                ex.HttpStatusCode,
                ex.StripeError?.Code);
            return Failure(CheckoutSessionProviderFailure.Unavailable);
        }
    }

    private static SessionCreateOptions ToStripeSessionCreateOptions(
        CheckoutSessionProviderRequest request,
        BillingStripeOptions stripeOptions)
    {
        var organizationId = request.OrganizationId.ToString();
        var metadata = new Dictionary<string, string>
        {
            [MetadataOrganizationId] = organizationId,
            [MetadataTargetPlanCode] = request.TargetPlanCode
        };

        var sessionOptions = new SessionCreateOptions
        {
            Mode = "subscription",
            SuccessUrl = stripeOptions.CheckoutSuccessUrl,
            CancelUrl = stripeOptions.CheckoutCancelUrl,
            ClientReferenceId = organizationId,
            Metadata = metadata,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = request.StripePriceId,
                    Quantity = 1
                }
            ],
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = metadata
            }
        };

        if (!string.IsNullOrWhiteSpace(request.ProviderCustomerId))
            sessionOptions.Customer = request.ProviderCustomerId;

        return sessionOptions;
    }

    private static Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>.Failure Failure(
        CheckoutSessionProviderFailure failure) => new(failure);
}
