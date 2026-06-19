using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an update technical service request status command from a REST resource.
/// </summary>
public static class UpdateTechnicalServiceRequestStatusCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateTechnicalServiceRequestStatusResource" /> into an
    ///     <see cref="UpdateTechnicalServiceRequestStatusCommand" />.
    /// </summary>
    public static UpdateTechnicalServiceRequestStatusCommand ToCommandFromResource(
        UpdateTechnicalServiceRequestStatusResource resource,
        int organizationId,
        int technicalServiceRequestId) =>
        new(organizationId,
            technicalServiceRequestId,
            resource.Status,
            resource.ClosureSummary,
            resource.Evidence,
            resource.ClosedBy);
}
