using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an assets query result.
/// </summary>
public static class ActionResultFromGetAssetsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetAssetsByOrganizationResult(
        Result<IEnumerable<Asset>, GetAssetsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<Asset>, GetAssetsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(AssetResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Asset>, GetAssetsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetAssetsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetAssetsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingAssets", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}