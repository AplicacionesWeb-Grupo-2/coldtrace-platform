using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Monitoring.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembles a demo generation command from a REST resource.
/// </summary>
public static class GenerateDemoSensorReadingsCommandFromResourceAssembler
{
    public static GenerateDemoSensorReadingsCommand ToCommandFromResource(
        GenerateDemoSensorReadingsResource? resource,
        int organizationId) =>
        new(
            organizationId,
            resource?.AssetId,
            resource?.Count);
}
