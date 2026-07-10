using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a technical service request creation result.
/// </summary>
public static class ActionResultFromCreateTechnicalServiceRequestResultAssembler
{
    /// <summary>
    ///     Converts a create technical service request result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromCreateTechnicalServiceRequestResult(
        Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    TechnicalServiceRequestResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Failure failure =>
                failure.Error switch
                {
                    CreateTechnicalServiceRequestError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateTechnicalServiceRequestError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    CreateTechnicalServiceRequestError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingTechnicalServiceRequest", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
