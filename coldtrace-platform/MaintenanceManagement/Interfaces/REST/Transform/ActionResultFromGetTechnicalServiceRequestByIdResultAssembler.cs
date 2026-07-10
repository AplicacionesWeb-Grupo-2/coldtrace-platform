using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a get technical service request by id result.
/// </summary>
public static class ActionResultFromGetTechnicalServiceRequestByIdResultAssembler
{
    /// <summary>
    ///     Converts a get technical service request by id result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromGetTechnicalServiceRequestByIdResult(
        Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Success success =>
                controller.Ok(TechnicalServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<TechnicalServiceRequest, GetTechnicalServiceRequestByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetTechnicalServiceRequestByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetTechnicalServiceRequestByIdAndOrganizationError.TechnicalServiceRequestNotFound =>
                        controller.ProblemResponse(localizer, "TechnicalServiceRequestNotFound", StatusCodes.Status404NotFound),
                    GetTechnicalServiceRequestByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingTechnicalServiceRequests", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
