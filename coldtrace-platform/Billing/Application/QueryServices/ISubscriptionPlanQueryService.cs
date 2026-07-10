using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Queries;

namespace ColdTrace.Platform.Billing.Application.QueryServices;

/// <summary>
///     Application service contract for subscription plan queries.
/// </summary>
public interface ISubscriptionPlanQueryService
{
    Task<IReadOnlyCollection<SubscriptionPlan>> Handle(
        GetActiveSubscriptionPlansQuery query,
        CancellationToken cancellationToken = default);
}
