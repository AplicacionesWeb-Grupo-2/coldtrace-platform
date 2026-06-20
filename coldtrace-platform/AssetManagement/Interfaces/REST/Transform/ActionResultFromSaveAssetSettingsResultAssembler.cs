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
                    SaveAssetSettingsError.OrganizationNotFound => controller.NotFound(localizer["OrganizationNotFound"].Value),
                    SaveAssetSettingsError.AssetNotFound => controller.NotFound(localizer["AssetNotFound"].Value),
                    SaveAssetSettingsError.UnexpectedError => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500),
                    _ => controller.BadRequest(localizer["InvalidRequest"].Value)
                },
            _ => controller.Problem()
        };
}
