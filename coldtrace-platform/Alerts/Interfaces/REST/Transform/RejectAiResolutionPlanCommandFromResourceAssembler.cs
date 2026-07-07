using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles a reject AI resolution plan command from a request resource.
/// </summary>
public static class RejectAiResolutionPlanCommandFromResourceAssembler
{
    public static RejectAiResolutionPlanCommand ToCommandFromResource(
        RejectAiResolutionPlanResource resource,
        int organizationId,
        int incidentId,
        int planId) =>
        new(
            organizationId,
            incidentId,
            planId,
            resource.RejectedBy,
            resource.RejectionReason);
}
