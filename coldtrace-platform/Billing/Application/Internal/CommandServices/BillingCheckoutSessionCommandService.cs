using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Application service implementation for provider-hosted checkout session creation.
/// </summary>
public class BillingCheckoutSessionCommandService(
    IOrganizationRepository organizationRepository,
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    ISubscriptionPlanQueryService subscriptionPlanQueryService,
    ICheckoutSessionProviderService checkoutSessionProviderService,
    ILogger<BillingCheckoutSessionCommandService> logger)
    : IBillingCheckoutSessionCommandService
{
    public async Task<Result<BillingCheckoutSession, BillingCheckoutSessionError>> Handle(
        CreateBillingCheckoutSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
            if (organization is null)
            {
                logger.LogWarning("Organization not found for checkout session: {OrganizationId}",
                    command.OrganizationId);
                return Failure(BillingCheckoutSessionError.OrganizationNotFound);
            }

            var subscription = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
                command.OrganizationId,
                cancellationToken);
            if (subscription is null)
            {
                logger.LogWarning("Organization subscription not found for checkout session: {OrganizationId}",
                    command.OrganizationId);
                return Failure(BillingCheckoutSessionError.OrganizationSubscriptionNotFound);
            }

            var plans = await subscriptionPlanQueryService.Handle(new GetActiveSubscriptionPlansQuery(), cancellationToken);
            var plan = plans.FirstOrDefault(activePlan =>
                string.Equals(activePlan.Code, command.TargetPlanCode, StringComparison.OrdinalIgnoreCase));
            if (plan is null || !plan.Active)
            {
                logger.LogWarning(
                    "Target plan not found for checkout session: {OrganizationId}, planCode={PlanCode}",
                    command.OrganizationId,
                    command.TargetPlanCode);
                return Failure(BillingCheckoutSessionError.TargetPlanNotFound);
            }

            if (!IsPaidPlan(plan))
            {
                logger.LogWarning("Free plan checkout rejected: {OrganizationId}, planCode={PlanCode}",
                    command.OrganizationId,
                    command.TargetPlanCode);
                return Failure(BillingCheckoutSessionError.FreePlanCheckoutNotAllowed);
            }

            if (plan.StripePriceId is null)
            {
                logger.LogWarning("Plan has no Stripe price id configured: {OrganizationId}, planCode={PlanCode}",
                    command.OrganizationId,
                    command.TargetPlanCode);
                return Failure(BillingCheckoutSessionError.PlanProviderPriceNotConfigured);
            }

            var providerResult = await checkoutSessionProviderService.CreateSubscriptionCheckoutSessionAsync(
                new CheckoutSessionProviderRequest(
                    command.OrganizationId,
                    plan.Code,
                    plan.StripePriceId,
                    subscription.ProviderCustomerId),
                cancellationToken);

            return providerResult switch
            {
                Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>.Success success =>
                    new Result<BillingCheckoutSession, BillingCheckoutSessionError>.Success(
                        new BillingCheckoutSession(
                            success.Value.Provider,
                            success.Value.SessionId,
                            success.Value.CheckoutUrl,
                            plan.Code)),
                Result<CheckoutSessionProviderResult, CheckoutSessionProviderFailure>.Failure failure =>
                    Failure(ToCommandError(failure.Error)),
                _ => Failure(BillingCheckoutSessionError.ProviderUnavailable)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating checkout session for organization {OrganizationId}",
                command.OrganizationId);
            return Failure(BillingCheckoutSessionError.UnexpectedError);
        }
    }

    private static Result<BillingCheckoutSession, BillingCheckoutSessionError>.Failure Failure(
        BillingCheckoutSessionError error) => new(error);

    private static bool IsPaidPlan(SubscriptionPlan plan) => plan.MonthlyPriceCents > 0;

    private static BillingCheckoutSessionError ToCommandError(CheckoutSessionProviderFailure failure) =>
        failure == CheckoutSessionProviderFailure.NotConfigured
            ? BillingCheckoutSessionError.ProviderNotConfigured
            : BillingCheckoutSessionError.ProviderUnavailable;
}
