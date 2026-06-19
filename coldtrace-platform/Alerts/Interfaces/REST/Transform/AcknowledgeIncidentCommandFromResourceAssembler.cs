using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an acknowledge incident command from a request resource.
/// </summary>
public static class AcknowledgeIncidentCommandFromResourceAssembler
{
    public static AcknowledgeIncidentCommand ToCommandFromResource(
        AcknowledgeIncidentResource resource,
        int organizationId,
        int incidentId) =>
        new(organizationId, incidentId, resource.AcknowledgedBy);
}
