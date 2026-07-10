using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a maintenance schedule status update result.
/// </summary>
public static class ActionResultFromUpdateMaintenanceScheduleStatusResultAssembler
{
    /// <summary>
    ///     Converts an update maintenance schedule status result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromUpdateMaintenanceScheduleStatusResult(
        Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Success success =>
                controller.Ok(MaintenanceScheduleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Failure failure =>
                failure.Error switch
                {
                    UpdateMaintenanceScheduleStatusError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateMaintenanceScheduleStatusError.MaintenanceScheduleNotFound =>
                        controller.ProblemResponse(localizer, "MaintenanceScheduleNotFound", StatusCodes.Status404NotFound),
                    UpdateMaintenanceScheduleStatusError.InvalidStatusTransition =>
                        controller.ProblemResponse(localizer, "MaintenanceScheduleInvalidTransition", StatusCodes.Status409Conflict),
                    UpdateMaintenanceScheduleStatusError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingMaintenanceSchedule", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
