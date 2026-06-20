using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles a corrective action command from a request resource.
/// </summary>
public static class RegisterIncidentCorrectiveActionCommandFromResourceAssembler
{
    public static RegisterIncidentCorrectiveActionCommand ToCommandFromResource(
        RegisterIncidentCorrectiveActionResource resource,
        int organizationId,
        int incidentId) =>
        new(organizationId, incidentId, resource.CorrectiveAction, resource.RegisteredBy);
}
