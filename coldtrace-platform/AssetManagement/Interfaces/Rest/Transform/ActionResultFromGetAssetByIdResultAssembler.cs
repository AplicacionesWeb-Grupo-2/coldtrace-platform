using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
﻿using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset-by-id query result.
/// </summary>
public static class ActionResultFromGetAssetByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetAssetByIdResult(
        Result<Asset, GetAssetByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<Asset, GetAssetByIdAndOrganizationError>.Success success =>
                controller.Ok(AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, GetAssetByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetAssetByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetAssetByIdAndOrganizationError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    GetAssetByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingAssets", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}