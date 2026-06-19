using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles a create technical service request command from a REST resource.
/// </summary>
public static class CreateTechnicalServiceRequestCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateTechnicalServiceRequestResource" /> into a
    ///     <see cref="CreateTechnicalServiceRequestCommand" />.
    /// </summary>
    public static CreateTechnicalServiceRequestCommand ToCommandFromResource(
        CreateTechnicalServiceRequestResource resource,
        int organizationId) =>
        new(organizationId,
            resource.AssetId,
            resource.IncidentId,
            resource.IssueDescription,
            resource.Priority,
            resource.RequestedBy);
}
