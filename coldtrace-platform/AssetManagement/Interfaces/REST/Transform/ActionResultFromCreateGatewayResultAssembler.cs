using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateway creation result.
/// </summary>
public static class ActionResultFromCreateGatewayResultAssembler
{
    /// <summary>
    ///     Converts a create gateway result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromCreateGatewayResult(
        Result<Gateway, CreateGatewayError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Gateway, CreateGatewayError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    GatewayResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Gateway, CreateGatewayError>.Failure failure =>
                failure.Error switch
                {
                    CreateGatewayError.DuplicateUuid => controller.ProblemResponse(localizer, "GatewayUuidDuplicated", StatusCodes.Status409Conflict),
                    CreateGatewayError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateGatewayError.LocationNotFound => controller.ProblemResponse(localizer, "LocationNotFound", StatusCodes.Status404NotFound),
                    CreateGatewayError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingGateway", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
