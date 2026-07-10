using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a location update result.
/// </summary>
public static class ActionResultFromUpdateLocationResultAssembler
{
    /// <summary>
    ///     Converts an update location result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromUpdateLocationResult(
        Result<Location, UpdateLocationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Location, UpdateLocationError>.Success success =>
                controller.Ok(LocationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Location, UpdateLocationError>.Failure failure =>
                failure.Error switch
                {
                    UpdateLocationError.DuplicateName => controller.ProblemResponse(localizer, "LocationNameDuplicated", StatusCodes.Status409Conflict),
                    UpdateLocationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateLocationError.LocationNotFound => controller.ProblemResponse(localizer, "LocationNotFound", StatusCodes.Status404NotFound),
                    UpdateLocationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingLocation", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
