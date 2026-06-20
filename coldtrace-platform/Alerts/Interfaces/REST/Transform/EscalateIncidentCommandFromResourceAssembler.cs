using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an escalation command from a request resource.
/// </summary>
public static class EscalateIncidentCommandFromResourceAssembler
{
    public static EscalateIncidentCommand ToCommandFromResource(
        EscalateIncidentResource resource,
        int organizationId,
        int incidentId) =>
        new(organizationId, incidentId, resource.EscalatedBy, resource.EscalationReason);
}
