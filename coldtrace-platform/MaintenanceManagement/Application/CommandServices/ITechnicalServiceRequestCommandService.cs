using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.MaintenanceManagement.Application.CommandServices;

/// <summary>
///     Contract for technical service request command operations.
/// </summary>
public interface ITechnicalServiceRequestCommandService
{
    Task<Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>> Handle(
        CreateTechnicalServiceRequestCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>> Handle(
        UpdateTechnicalServiceRequestStatusCommand command,
        CancellationToken cancellationToken = default);
}
