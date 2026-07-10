using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a get maintenance schedule by id result.
/// </summary>
public static class ActionResultFromGetMaintenanceScheduleByIdResultAssembler
{
    /// <summary>
    ///     Converts a get maintenance schedule by id result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromGetMaintenanceScheduleByIdResult(
        Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<MaintenanceManagementMessages> localizer) =>
        result switch
        {
            Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Success success =>
                controller.Ok(MaintenanceScheduleResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<MaintenanceSchedule, GetMaintenanceScheduleByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetMaintenanceScheduleByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetMaintenanceScheduleByIdAndOrganizationError.MaintenanceScheduleNotFound =>
                        controller.ProblemResponse(localizer, "MaintenanceScheduleNotFound", StatusCodes.Status404NotFound),
                    GetMaintenanceScheduleByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingMaintenanceSchedules", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
