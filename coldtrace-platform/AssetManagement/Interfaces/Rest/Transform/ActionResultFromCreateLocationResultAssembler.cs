using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a location creation result.
/// </summary>
public static class ActionResultFromCreateLocationResultAssembler
{
    /// <summary>
    ///     Converts a create location result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromCreateLocationResult(
        Result<Location, CreateLocationError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<Location, CreateLocationError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    LocationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Location, CreateLocationError>.Failure failure =>
                failure.Error switch
                {
                    CreateLocationError.DuplicateName => controller.ProblemResponse(localizer, "LocationNameDuplicated", StatusCodes.Status409Conflict),
                    CreateLocationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateLocationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingLocation", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
