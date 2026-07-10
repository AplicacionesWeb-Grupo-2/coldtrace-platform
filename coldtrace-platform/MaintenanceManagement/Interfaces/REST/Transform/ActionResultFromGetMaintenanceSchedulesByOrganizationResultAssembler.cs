using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a get maintenance schedules by organization result.
/// </summary>
public static class ActionResultFromGetMaintenanceSchedulesByOrganizationResultAssembler
{
    /// <summary>
    ///     Converts a get maintenance schedules by organization result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromGetMaintenanceSchedulesByOrganizationResult(
        Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(
                    MaintenanceScheduleResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<MaintenanceSchedule>, GetMaintenanceSchedulesByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetMaintenanceSchedulesByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetMaintenanceSchedulesByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingMaintenanceSchedules", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
