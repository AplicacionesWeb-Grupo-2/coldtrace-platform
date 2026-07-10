using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateway update result.
/// </summary>
public static class ActionResultFromUpdateGatewayResultAssembler
{
    /// <summary>
    ///     Converts an update gateway result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromUpdateGatewayResult(
        Result<Gateway, UpdateGatewayError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<Gateway, UpdateGatewayError>.Success success =>
                controller.Ok(GatewayResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Gateway, UpdateGatewayError>.Failure failure =>
                failure.Error switch
                {
                    UpdateGatewayError.DuplicateUuid => controller.ProblemResponse(localizer, "GatewayUuidDuplicated", StatusCodes.Status409Conflict),
                    UpdateGatewayError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateGatewayError.LocationNotFound => controller.ProblemResponse(localizer, "LocationNotFound", StatusCodes.Status404NotFound),
                    UpdateGatewayError.GatewayNotFound => controller.ProblemResponse(localizer, "GatewayNotFound", StatusCodes.Status404NotFound),
                    UpdateGatewayError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingGateway", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
