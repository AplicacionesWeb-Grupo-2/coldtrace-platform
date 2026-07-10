using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Assembles HTTP action results from subscription plan query results.
/// </summary>
public static class ActionResultFromSubscriptionPlanQueryResultAssembler
{
    public static ActionResult ToActionResultFromList(
        IReadOnlyCollection<SubscriptionPlan> plans,
        ControllerBase controller) =>
        controller.Ok(plans.Select(SubscriptionPlanResourceFromEntityAssembler.ToResourceFromEntity));
}
