using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Internal.Services;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Application.Internal.QueryServices;

/// <summary>
///     Application service implementation for organization subscription queries.
/// </summary>
public class OrganizationSubscriptionQueryService(
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    IOrganizationRepository organizationRepository,
    ISubscriptionPlanQueryService subscriptionPlanQueryService,
    OrganizationSubscriptionUsageService usageService,
    EntitlementPolicyService entitlementPolicyService,
    ILogger<OrganizationSubscriptionQueryService> logger)
    : IOrganizationSubscriptionQueryService
{
    public async Task<Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>> Handle(
        GetOrganizationSubscriptionByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
            if (organization is null)
            {
                logger.LogWarning("Organization not found for subscription query: {OrganizationId}",
                    query.OrganizationId);
                return new Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Failure(
                    GetOrganizationSubscriptionError.OrganizationNotFound);
            }

            var subscription = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            if (subscription is null)
            {
                logger.LogWarning("Organization subscription not found: {OrganizationId}", query.OrganizationId);
                return new Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Failure(
                    GetOrganizationSubscriptionError.OrganizationSubscriptionNotFound);
            }

            var plans = await subscriptionPlanQueryService.Handle(new GetActiveSubscriptionPlansQuery(), cancellationToken);
            var plan = plans.FirstOrDefault(activePlan =>
                string.Equals(activePlan.Code, subscription.PlanCode, StringComparison.OrdinalIgnoreCase));
            if (plan is null)
            {
                logger.LogWarning(
                    "Subscription plan not found for organization subscription: {OrganizationId}, planCode={PlanCode}",
                    query.OrganizationId,
                    subscription.PlanCode);
                return new Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Failure(
                    GetOrganizationSubscriptionError.SubscriptionPlanNotFound);
            }

            var usage = await usageService.SnapshotForAsync(query.OrganizationId, cancellationToken);
            var entitlements = entitlementPolicyService.Compute(subscription, plan, usage);
            return new Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Success(
                new OrganizationSubscriptionDetails(subscription, plan, usage, entitlements));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying subscription for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>.Failure(
                GetOrganizationSubscriptionError.UnexpectedError);
        }
    }
}
