using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a corrective action registration result.
/// </summary>
public static class ActionResultFromRegisterIncidentCorrectiveActionResultAssembler
{
    public static ActionResult ToActionResultFromRegisterIncidentCorrectiveActionResult(
        Result<Incident, RegisterIncidentCorrectiveActionError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Incident, RegisterIncidentCorrectiveActionError>.Success success =>
                controller.Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Incident, RegisterIncidentCorrectiveActionError>.Failure failure =>
                failure.Error switch
                {
                    RegisterIncidentCorrectiveActionError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    RegisterIncidentCorrectiveActionError.IncidentNotFound =>
                        controller.ProblemResponse(localizer, "IncidentNotFound", StatusCodes.Status404NotFound),
                    RegisterIncidentCorrectiveActionError.AlreadyResolved =>
                        controller.ProblemResponse(localizer, "IncidentAlreadyResolved", StatusCodes.Status409Conflict),
                    RegisterIncidentCorrectiveActionError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorRegisteringIncidentCorrectiveAction", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
