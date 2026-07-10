using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Infrastructure.Configuration;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.BillingPortal;

namespace ColdTrace.Platform.Billing.Infrastructure.Stripe;

/// <summary>
///     Stripe-backed customer portal session provider adapter.
/// </summary>
public class StripePortalSessionProviderService(
    IOptions<BillingOptions> options,
    ILogger<StripePortalSessionProviderService> logger)
    : IPortalSessionProviderService
{
    public async Task<Result<PortalSessionProviderResult, PortalSessionProviderFailure>>
        CreateCustomerPortalSessionAsync(
            PortalSessionProviderRequest request,
            CancellationToken cancellationToken = default)
    {
        var stripeOptions = options.Value.Stripe;
        if (!stripeOptions.HasCustomerPortalConfiguration())
        {
            logger.LogWarning("Stripe customer portal configuration is incomplete");
            return Failure(PortalSessionProviderFailure.NotConfigured);
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
                logger.LogWarning("Stripe customer portal session did not include a redirect URL: {SessionId}",
                    session.Id);
                return Failure(PortalSessionProviderFailure.Unavailable);
            }

            return new Result<PortalSessionProviderResult, PortalSessionProviderFailure>.Success(
                new PortalSessionProviderResult(BillingProviders.Stripe, session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex,
                "Stripe customer portal session creation failed: organizationId={OrganizationId}, status={Status}, code={Code}",
                request.OrganizationId,
                ex.HttpStatusCode,
                ex.StripeError?.Code);
            return Failure(PortalSessionProviderFailure.Unavailable);
        }
    }

    private static SessionCreateOptions ToStripeSessionCreateOptions(
        PortalSessionProviderRequest request,
        BillingStripeOptions stripeOptions) =>
        new()
        {
            Customer = request.ProviderCustomerId,
            ReturnUrl = stripeOptions.CustomerPortalReturnUrl
        };

    private static Result<PortalSessionProviderResult, PortalSessionProviderFailure>.Failure Failure(
        PortalSessionProviderFailure failure) => new(failure);
}
