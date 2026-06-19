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
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    UpdateTechnicalServiceRequestStatusError.TechnicalServiceRequestNotFound =>
                        controller.NotFound(localizer["TechnicalServiceRequestNotFound"].Value),
                    UpdateTechnicalServiceRequestStatusError.InvalidStatusTransition =>
                        controller.Conflict(localizer["TechnicalServiceRequestInvalidTransition"].Value),
                    UpdateTechnicalServiceRequestStatusError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorUpdatingTechnicalServiceRequest"].Value,
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
