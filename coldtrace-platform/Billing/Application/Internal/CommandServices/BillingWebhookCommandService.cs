using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Model.Entities;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Application service implementation for signed billing provider webhook processing.
/// </summary>
public class BillingWebhookCommandService(
    IBillingWebhookProviderService billingWebhookProviderService,
    IBillingWebhookEventRepository billingWebhookEventRepository,
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    ISubscriptionPlanQueryService subscriptionPlanQueryService,
    IUnitOfWork unitOfWork,
    ILogger<BillingWebhookCommandService> logger)
    : IBillingWebhookCommandService
{
    private const string BasePlanCode = "base";

    public async Task<Result<BillingWebhookProcessingResult, BillingWebhookError>> Handle(
        ProcessStripeWebhookCommand command,
        CancellationToken cancellationToken = default)
    {
        var providerResult = await billingWebhookProviderService.ParseSignedEventAsync(
            command.Payload,
            command.SignatureHeader,
            cancellationToken);

        return providerResult switch
        {
            Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>.Success success =>
                await ProcessVerifiedEventAsync(success.Value, cancellationToken),
            Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>.Failure failure =>
                Failure(ToCommandError(failure.Error)),
            _ => Failure(BillingWebhookError.ProcessingFailed)
        };
    }

    private async Task<Result<BillingWebhookProcessingResult, BillingWebhookError>> ProcessVerifiedEventAsync(
        BillingWebhookProviderEvent webhookEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            if (await billingWebhookEventRepository.ExistsByProviderAndEventIdAsync(
                    webhookEvent.Provider,
                    webhookEvent.EventId,
                    cancellationToken))
            {
                logger.LogInformation(
                    "Stripe webhook event already processed: eventId={EventId}, eventType={EventType}",
                    webhookEvent.EventId,
                    webhookEvent.EventType);
                return Success(
                    webhookEvent,
                    BillingWebhookEventStatuses.Processed,
                    true,
                    webhookEvent.OrganizationId,
                    null,
                    null);
            }

            if (!webhookEvent.Supported)
            {
                await SaveWebhookEventAsync(webhookEvent, BillingWebhookEventStatuses.Ignored, null, cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogDebug("Stripe webhook event ignored: eventId={EventId}, eventType={EventType}",
                    webhookEvent.EventId,
                    webhookEvent.EventType);
                return Success(webhookEvent, BillingWebhookEventStatuses.Ignored, false, null, null, null);
            }

            var subscription = await FindTargetSubscriptionAsync(webhookEvent, cancellationToken);
            if (subscription is null)
            {
                await SaveWebhookEventAsync(
                    webhookEvent,
                    BillingWebhookEventStatuses.Ignored,
                    "subscription-not-found",
                    cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogWarning(
                    "Stripe webhook had no matching organization subscription: eventId={EventId}, eventType={EventType}",
                    webhookEvent.EventId,
                    webhookEvent.EventType);
                return Success(
                    webhookEvent,
                    BillingWebhookEventStatuses.Ignored,
                    false,
                    webhookEvent.OrganizationId,
                    null,
                    null);
            }

            var plan = await ResolvePlanAsync(webhookEvent, subscription, cancellationToken);
            if (plan is null)
            {
                await SaveWebhookEventAsync(webhookEvent, BillingWebhookEventStatuses.Ignored, "plan-not-found",
                    cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogWarning(
                    "Stripe webhook had no matching local plan: eventId={EventId}, eventType={EventType}, targetPlanCode={PlanCode}, priceId={PriceId}",
                    webhookEvent.EventId,
                    webhookEvent.EventType,
                    webhookEvent.TargetPlanCode,
                    webhookEvent.StripePriceId);
                return Success(
                    webhookEvent,
                    BillingWebhookEventStatuses.Ignored,
                    false,
                    subscription.OrganizationId,
                    null,
                    null);
            }

            var updatedSubscription = SynchronizeSubscription(subscription, plan, webhookEvent);
            await SaveWebhookEventAsync(webhookEvent, BillingWebhookEventStatuses.Processed, null, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            logger.LogInformation(
                "Stripe webhook synchronized subscription: organizationId={OrganizationId}, eventId={EventId}, eventType={EventType}, planCode={PlanCode}, status={Status}",
                updatedSubscription.OrganizationId,
                webhookEvent.EventId,
                webhookEvent.EventType,
                updatedSubscription.PlanCode,
                updatedSubscription.Status);
            return Success(
                webhookEvent,
                BillingWebhookEventStatuses.Processed,
                false,
                updatedSubscription.OrganizationId,
                updatedSubscription.PlanCode,
                updatedSubscription.Status);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error processing Stripe webhook: eventId={EventId}, eventType={EventType}",
                webhookEvent.EventId,
                webhookEvent.EventType);
            return Failure(BillingWebhookError.ProcessingFailed);
        }
    }

    private async Task<OrganizationSubscription?> FindTargetSubscriptionAsync(
        BillingWebhookProviderEvent webhookEvent,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(webhookEvent.ProviderSubscriptionId))
        {
            var subscription = await organizationSubscriptionRepository.FindByProviderSubscriptionIdAsync(
                webhookEvent.ProviderSubscriptionId,
                cancellationToken);
            if (subscription is not null) return subscription;
        }

        if (!string.IsNullOrWhiteSpace(webhookEvent.ProviderCustomerId))
        {
            var subscription = await organizationSubscriptionRepository.FindByProviderCustomerIdAsync(
                webhookEvent.ProviderCustomerId,
                cancellationToken);
            if (subscription is not null) return subscription;
        }

        return webhookEvent.OrganizationId is { } organizationId
            ? await organizationSubscriptionRepository.FindByOrganizationIdAsync(organizationId, cancellationToken)
            : null;
    }

    private async Task<SubscriptionPlan?> ResolvePlanAsync(
        BillingWebhookProviderEvent webhookEvent,
        OrganizationSubscription subscription,
        CancellationToken cancellationToken)
    {
        var plans = await subscriptionPlanQueryService.Handle(new GetActiveSubscriptionPlansQuery(), cancellationToken);

        if (!string.IsNullOrWhiteSpace(webhookEvent.TargetPlanCode))
        {
            var plan = plans.FirstOrDefault(activePlan =>
                string.Equals(activePlan.Code, webhookEvent.TargetPlanCode, StringComparison.OrdinalIgnoreCase));
            if (plan is not null) return plan;
        }

        if (!string.IsNullOrWhiteSpace(webhookEvent.StripePriceId))
        {
            var plan = plans.FirstOrDefault(activePlan =>
                string.Equals(activePlan.StripePriceId, webhookEvent.StripePriceId, StringComparison.Ordinal));
            if (plan is not null) return plan;
        }

        return plans.FirstOrDefault(activePlan =>
                   string.Equals(activePlan.Code, subscription.PlanCode, StringComparison.OrdinalIgnoreCase))
               ?? plans.FirstOrDefault(activePlan =>
                   string.Equals(activePlan.Code, BasePlanCode, StringComparison.OrdinalIgnoreCase));
    }

    private OrganizationSubscription SynchronizeSubscription(
        OrganizationSubscription subscription,
        SubscriptionPlan plan,
        BillingWebhookProviderEvent webhookEvent)
    {
        subscription.SynchronizeProviderState(
            plan.Code,
            webhookEvent.SubscriptionStatus,
            webhookEvent.Provider,
            FirstNonBlank(webhookEvent.ProviderCustomerId, subscription.ProviderCustomerId),
            FirstNonBlank(webhookEvent.ProviderSubscriptionId, subscription.ProviderSubscriptionId),
            webhookEvent.CurrentPeriodStart,
            webhookEvent.CurrentPeriodEnd,
            webhookEvent.CancelAtPeriodEnd,
            MetadataFor(webhookEvent));
        organizationSubscriptionRepository.Update(subscription);
        return subscription;
    }

    private async Task SaveWebhookEventAsync(
        BillingWebhookProviderEvent webhookEvent,
        string status,
        string? reason,
        CancellationToken cancellationToken)
    {
        await billingWebhookEventRepository.AddAsync(
            new BillingWebhookEvent(
                webhookEvent.Provider,
                webhookEvent.EventId,
                webhookEvent.EventType,
                status,
                webhookEvent.OrganizationId,
                webhookEvent.ProviderCustomerId,
                webhookEvent.ProviderSubscriptionId,
                DateTimeOffset.UtcNow,
                reason is null ? MetadataFor(webhookEvent) : $"{MetadataFor(webhookEvent)}; reason={reason}"),
            cancellationToken);
    }

    private static Result<BillingWebhookProcessingResult, BillingWebhookError>.Success Success(
        BillingWebhookProviderEvent webhookEvent,
        string processingStatus,
        bool duplicate,
        int? organizationId,
        string? planCode,
        string? subscriptionStatus) =>
        new(new BillingWebhookProcessingResult(
            webhookEvent.Provider,
            webhookEvent.EventId,
            webhookEvent.EventType,
            processingStatus,
            duplicate,
            organizationId,
            planCode,
            subscriptionStatus));

    private static Result<BillingWebhookProcessingResult, BillingWebhookError>.Failure Failure(
        BillingWebhookError error) => new(error);

    private static string MetadataFor(BillingWebhookProviderEvent webhookEvent) =>
        $"objectId={Safe(webhookEvent.ObjectId)}; priceId={Safe(webhookEvent.StripePriceId)}";

    private static string? FirstNonBlank(string? first, string? second) =>
        !string.IsNullOrWhiteSpace(first) ? first : second;

    private static string Safe(string? value) => value is null ? "none" : value;

    private static BillingWebhookError ToCommandError(BillingWebhookProviderFailure failure) =>
        failure switch
        {
            BillingWebhookProviderFailure.NotConfigured => BillingWebhookError.ProviderNotConfigured,
            BillingWebhookProviderFailure.MissingSignature => BillingWebhookError.MissingSignature,
            BillingWebhookProviderFailure.InvalidSignature => BillingWebhookError.InvalidSignature,
            BillingWebhookProviderFailure.InvalidPayload => BillingWebhookError.InvalidPayload,
            _ => BillingWebhookError.ProcessingFailed
        };
}
