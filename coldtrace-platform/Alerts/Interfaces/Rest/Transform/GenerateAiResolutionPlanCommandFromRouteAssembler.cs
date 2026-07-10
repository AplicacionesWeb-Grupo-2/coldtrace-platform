using ColdTrace.Platform.Alerts.Domain.Model.Commands;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an AI resolution plan generation command from route values.
/// </summary>
public static class GenerateAiResolutionPlanCommandFromRouteAssembler
{
    public static GenerateAiResolutionPlanCommand ToCommandFromRoute(int organizationId, int incidentId) =>
        new(organizationId, incidentId);
}
