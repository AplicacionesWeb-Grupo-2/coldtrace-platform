using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an approve AI resolution plan command from a request resource.
/// </summary>
public static class ApproveAiResolutionPlanCommandFromResourceAssembler
{
    public static ApproveAiResolutionPlanCommand ToCommandFromResource(
        ApproveAiResolutionPlanResource resource,
        int organizationId,
        int incidentId,
        int planId) =>
        new(
            organizationId,
            incidentId,
            planId,
            resource.ApprovedBy,
            resource.FinalCorrectiveAction,
            resource.FinalResolutionNotes);
}
