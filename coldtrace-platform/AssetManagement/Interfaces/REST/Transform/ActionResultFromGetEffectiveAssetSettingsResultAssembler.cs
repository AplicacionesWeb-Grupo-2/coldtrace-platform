using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Converts effective asset settings query results to action results.
/// </summary>
public static class ActionResultFromGetEffectiveAssetSettingsResultAssembler
{
    public static ActionResult ToActionResultFromGetEffectiveAssetSettingsResult(
        Result<AssetSettings, GetEffectiveAssetSettingsError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AssetSettings, GetEffectiveAssetSettingsError>.Success success =>
                controller.Ok(AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<AssetSettings, GetEffectiveAssetSettingsError>.Failure failure =>
                failure.Error switch
                {
                    GetEffectiveAssetSettingsError.AssetNotFound => controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    GetEffectiveAssetSettingsError.AssetSettingsNotFound => controller.ProblemResponse(localizer, "ResourceNotFound", StatusCodes.Status404NotFound),
                    GetEffectiveAssetSettingsError.UnexpectedError => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError),
                    _ => controller.ValidationProblemResponse(localizer, "InvalidRequest")
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };
}
