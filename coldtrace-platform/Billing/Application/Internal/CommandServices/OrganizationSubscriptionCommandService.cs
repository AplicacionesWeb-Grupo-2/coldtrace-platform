using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Application service implementation for organization subscription commands.
/// </summary>
public class OrganizationSubscriptionCommandService(
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    IOrganizationRepository organizationRepository,
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
        var organizations = await organizationRepository.ListAsync(cancellationToken);
        var created = 0;

        foreach (var organization in organizations)
        {
            var existing = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
                organization.Id,
                cancellationToken);
            if (existing is not null) continue;

            await organizationSubscriptionRepository.AddAsync(
                new OrganizationSubscription(organization.Id),
                cancellationToken);
            created++;
        }

        if (created > 0)
            await unitOfWork.CompleteAsync(cancellationToken);

        logger.LogDebug("Verified Base subscriptions for {OrganizationCount} organizations; created {CreatedCount}",
            organizations.Count(),
            created);
    }
}
