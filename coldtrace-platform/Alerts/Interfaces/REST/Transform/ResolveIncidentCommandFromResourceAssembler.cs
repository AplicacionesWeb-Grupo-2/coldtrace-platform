using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles a resolve incident command from a request resource.
/// </summary>
public static class ResolveIncidentCommandFromResourceAssembler
{
    public static ResolveIncidentCommand ToCommandFromResource(
        ResolveIncidentResource resource,
        int organizationId,
        int incidentId) =>
        new(organizationId, incidentId, resource.ResolvedBy, resource.ResolutionNotes);
}
