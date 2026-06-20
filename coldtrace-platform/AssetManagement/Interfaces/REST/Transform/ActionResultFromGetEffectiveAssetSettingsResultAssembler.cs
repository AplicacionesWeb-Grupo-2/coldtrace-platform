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
                    GetEffectiveAssetSettingsError.AssetNotFound => controller.NotFound(localizer["AssetNotFound"].Value),
                    GetEffectiveAssetSettingsError.AssetSettingsNotFound => controller.NotFound(localizer["ResourceNotFound"].Value),
                    GetEffectiveAssetSettingsError.UnexpectedError => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500),
                    _ => controller.BadRequest(localizer["InvalidRequest"].Value)
                },
            _ => controller.Problem()
        };
}
