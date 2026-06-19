using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.MaintenanceManagement.Domain.Services;

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
