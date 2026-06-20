using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a maintenance schedule creation result.
/// </summary>
public static class ActionResultFromCreateMaintenanceScheduleResultAssembler
{
    /// <summary>
    ///     Converts a create maintenance schedule result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromCreateMaintenanceScheduleResult(
        Result<MaintenanceSchedule, CreateMaintenanceScheduleError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    MaintenanceScheduleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Failure failure =>
                failure.Error switch
                {
                    CreateMaintenanceScheduleError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    CreateMaintenanceScheduleError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    CreateMaintenanceScheduleError.DuplicateActiveSchedule =>
                        controller.Conflict(localizer["MaintenanceScheduleDuplicateActive"].Value),
                    CreateMaintenanceScheduleError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingMaintenanceSchedule"].Value,
                            statusCode: 500),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
