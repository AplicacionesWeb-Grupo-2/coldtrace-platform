using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Reports.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles report generation commands from REST resources.
/// </summary>
public static class GenerateReportCommandFromResourceAssembler
{
    public static GenerateReportCommand ToCommandFromResource(
        GenerateReportResource resource,
        int organizationId) =>
        new(
            organizationId,
            resource.Type,
            resource.Title,
            resource.PeriodStart,
            resource.PeriodEnd);
}
