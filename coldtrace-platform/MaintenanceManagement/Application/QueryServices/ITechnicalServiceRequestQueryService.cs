using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.MaintenanceManagement.Application.QueryServices;

/// <summary>
///     Contract for technical service request query operations.
/// </summary>
public interface ITechnicalServiceRequestQueryService
{
    Task<Result<IEnumerable<TechnicalServiceRequest>, GetTechnicalServiceRequestsByOrganizationError>> Handle(
        GetTechnicalServiceRequestsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>> Handle(
        GetTechnicalServiceRequestByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
