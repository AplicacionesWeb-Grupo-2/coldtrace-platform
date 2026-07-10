using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Converts asset settings save results to action results.
/// </summary>
public static class ActionResultFromSaveAssetSettingsResultAssembler
{
    public static ActionResult ToActionResultFromSaveAssetSettingsResult(
        Result<AssetSettings, SaveAssetSettingsError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AssetSettings, SaveAssetSettingsError>.Success success =>
                controller.Ok(AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<AssetSettings, SaveAssetSettingsError>.Failure failure =>
                failure.Error switch
                {
                    SaveAssetSettingsError.OrganizationNotFound => controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    SaveAssetSettingsError.AssetNotFound => controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    SaveAssetSettingsError.UnexpectedError => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError),
                    _ => controller.ValidationProblemResponse(localizer, "InvalidRequest")
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };
}
