using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a corrective action registration result.
/// </summary>
public static class ActionResultFromRegisterIncidentCorrectiveActionResultAssembler
{
    public static ActionResult ToActionResultFromRegisterIncidentCorrectiveActionResult(
        Result<Incident, RegisterIncidentCorrectiveActionError> result,
        ControllerBase controller,
        IStringLocalizer<AlertsMessages> localizer) =>
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
