using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a technical service request status update result.
/// </summary>
public static class ActionResultFromUpdateTechnicalServiceRequestStatusResultAssembler
{
    /// <summary>
    ///     Converts an update technical service request status result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromUpdateTechnicalServiceRequestStatusResult(
        Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Success success =>
                controller.Ok(TechnicalServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Failure failure =>
                failure.Error switch
                {
                    UpdateTechnicalServiceRequestStatusError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateTechnicalServiceRequestStatusError.TechnicalServiceRequestNotFound =>
                        controller.ProblemResponse(localizer, "TechnicalServiceRequestNotFound", StatusCodes.Status404NotFound),
                    UpdateTechnicalServiceRequestStatusError.InvalidStatusTransition =>
                        controller.ProblemResponse(localizer, "TechnicalServiceRequestInvalidTransition", StatusCodes.Status409Conflict),
                    UpdateTechnicalServiceRequestStatusError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingTechnicalServiceRequest", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
