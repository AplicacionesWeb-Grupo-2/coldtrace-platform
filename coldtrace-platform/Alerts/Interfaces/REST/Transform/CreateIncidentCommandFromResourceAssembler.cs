using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles a create incident command from a request resource.
/// </summary>
public static class CreateIncidentCommandFromResourceAssembler
{
    public static CreateIncidentCommand ToCommandFromResource(CreateIncidentResource resource, int organizationId) =>
        new(
            organizationId,
            resource.AssetId,
            resource.DeviceId,
            resource.ReadingId,
            resource.AssetName,
            resource.DeviceName,
            resource.Type,
            resource.Severity,
            resource.Value);
}
