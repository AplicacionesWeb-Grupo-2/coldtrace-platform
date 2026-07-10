using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset creation result.
/// </summary>
public static class ActionResultFromCreateAssetResultAssembler
{
    public static ActionResult ToActionResultFromCreateAssetResult(
        Result<Asset, CreateAssetError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Asset, CreateAssetError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, CreateAssetError>.Failure failure =>
                failure.Error switch
                {
                    CreateAssetError.DuplicateUuid =>
                        controller.ProblemResponse(localizer, "AssetUuidDuplicated", StatusCodes.Status409Conflict),
                    CreateAssetError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateAssetError.LocationNotFound =>
                        controller.ProblemResponse(localizer, "LocationNotFound", StatusCodes.Status404NotFound),
                    CreateAssetError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingAsset", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}