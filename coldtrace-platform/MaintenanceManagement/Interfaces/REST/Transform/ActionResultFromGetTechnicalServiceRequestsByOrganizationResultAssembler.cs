using ColdTrace.Platform.MaintenanceManagement.Application.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a get technical service requests by organization result.
/// </summary>
public static class ActionResultFromGetTechnicalServiceRequestsByOrganizationResultAssembler
{
    /// <summary>
    ///     Converts a get technical service requests result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromGetTechnicalServiceRequestsByOrganizationResult(
        Result<IEnumerable<TechnicalServiceRequest>, GetTechnicalServiceRequestsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<TechnicalServiceRequest>, GetTechnicalServiceRequestsByOrganizationError>.Success
                success =>
                controller.Ok(success.Value.Select(
                    TechnicalServiceRequestResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<TechnicalServiceRequest>, GetTechnicalServiceRequestsByOrganizationError>.Failure
                failure =>
                failure.Error switch
                {
                    GetTechnicalServiceRequestsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetTechnicalServiceRequestsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingTechnicalServiceRequests", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
