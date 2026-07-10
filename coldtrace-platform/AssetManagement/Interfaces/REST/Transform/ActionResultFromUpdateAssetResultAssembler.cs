using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset update result.
/// </summary>
public static class ActionResultFromUpdateAssetResultAssembler
{
    public static ActionResult ToActionResultFromUpdateAssetResult(
        Result<Asset, UpdateAssetError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Asset, UpdateAssetError>.Success success =>
                controller.Ok(AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, UpdateAssetError>.Failure failure =>
                failure.Error switch
                {
                    UpdateAssetError.DuplicateUuid =>
                        controller.ProblemResponse(localizer, "AssetUuidDuplicated", StatusCodes.Status409Conflict),
                    UpdateAssetError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateAssetError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    UpdateAssetError.LocationNotFound =>
                        controller.ProblemResponse(localizer, "LocationNotFound", StatusCodes.Status404NotFound),
                    UpdateAssetError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingAsset", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}