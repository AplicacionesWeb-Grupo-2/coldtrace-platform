using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Commands;

namespace ColdTrace.Platform.Billing.Application.CommandServices;

/// <summary>
///     Application service contract for organization subscription commands.
/// </summary>
public interface IOrganizationSubscriptionCommandService
{
    Task<OrganizationSubscription> Handle(
        InitializeBaseOrganizationSubscriptionCommand command,
        CancellationToken cancellationToken = default);

    Task Handle(
        SeedBaseOrganizationSubscriptionsCommand command,
        CancellationToken cancellationToken = default);
}
