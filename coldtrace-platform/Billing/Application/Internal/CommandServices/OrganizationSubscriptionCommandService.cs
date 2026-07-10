using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Application.CommandServices;
using ColdTrace.Platform.Billing.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Application service implementation for organization subscription commands.
/// </summary>
public class OrganizationSubscriptionCommandService(
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<OrganizationSubscriptionCommandService> logger)
    : IOrganizationSubscriptionCommandService
{
    public async Task<OrganizationSubscription> Handle(
        InitializeBaseOrganizationSubscriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
            command.OrganizationId,
            cancellationToken);
        if (existing is not null) return existing;

        var subscription = new OrganizationSubscription(command.OrganizationId);
        await organizationSubscriptionRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("Initialized Base subscription for organization {OrganizationId}",
            command.OrganizationId);
        return subscription;
    }

    public async Task Handle(
        SeedBaseOrganizationSubscriptionsCommand command,
        CancellationToken cancellationToken = default)
    {
        var organizationIds = await iamContextFacade.ListOrganizationIdsAsync(cancellationToken);
        var created = 0;

        foreach (var organizationId in organizationIds)
        {
            var existing = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
                organizationId,
                cancellationToken);
            if (existing is not null) continue;

            await organizationSubscriptionRepository.AddAsync(
                new OrganizationSubscription(organizationId),
                cancellationToken);
            created++;
        }

        if (created > 0)
            await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogDebug("Verified Base subscriptions for {OrganizationCount} organizations; created {CreatedCount}",
            organizationIds.Count(),
            created);
    }
}
