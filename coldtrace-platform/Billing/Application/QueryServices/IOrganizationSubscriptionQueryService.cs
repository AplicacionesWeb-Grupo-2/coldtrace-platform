using ColdTrace.Platform.Billing.Domain.Model.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Billing.Application.QueryServices;

/// <summary>
///     Application service contract for organization subscription queries.
/// </summary>
public interface IOrganizationSubscriptionQueryService
{
    Task<Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>> Handle(
        GetOrganizationSubscriptionByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
