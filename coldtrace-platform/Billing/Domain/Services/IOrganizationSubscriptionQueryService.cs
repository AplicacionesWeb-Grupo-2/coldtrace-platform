using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Domain.Services;

/// <summary>
///     Application service contract for organization subscription queries.
/// </summary>
public interface IOrganizationSubscriptionQueryService
{
    Task<Result<OrganizationSubscriptionDetails, GetOrganizationSubscriptionError>> Handle(
        GetOrganizationSubscriptionByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
