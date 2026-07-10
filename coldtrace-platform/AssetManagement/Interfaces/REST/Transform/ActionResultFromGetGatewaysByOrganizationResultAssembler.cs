using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateways query result.
/// </summary>
public static class ActionResultFromGetGatewaysByOrganizationResultAssembler
{
    /// <summary>
    ///     Converts a gateways-by-organization result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromGetGatewaysByOrganizationResult(
        Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(GatewayResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Gateway>, GetGatewaysByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetGatewaysByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetGatewaysByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingGateways", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
