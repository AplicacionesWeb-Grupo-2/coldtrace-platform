using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST;

/// <summary>
///     Asset settings controller.
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Asset Settings")]
public class AssetSettingsController(
    IAssetSettingsCommandService assetSettingsCommandService,
    IAssetSettingsQueryService assetSettingsQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<AssetSettingsController> logger)
    : ControllerBase
{
    [HttpGet("api/v1/organizations/{organizationId:int}/asset-settings")]
    [SwaggerOperation(
        Summary = "Gets asset settings by organization",
        Description = "Gets all asset settings that belong to the provided organization",
        OperationId = "GetAssetSettingsByOrganization")]
    [SwaggerResponse(200, "Asset settings found", typeof(IEnumerable<AssetSettingsResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetAssetSettingsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await assetSettingsQueryService.Handle(
            new GetAssetSettingsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetAssetSettingsByOrganizationResultAssembler
            .ToActionResultFromGetAssetSettingsByOrganizationResult(result, this, localizer);
    }

    [HttpGet("api/v1/organizations/{organizationId:int}/assets/{assetId:int}/settings")]
    [SwaggerOperation(
        Summary = "Gets effective asset settings for an asset",
        Description = "Gets asset-specific settings or falls back to organization default",
        OperationId = "GetEffectiveAssetSettingsByAsset")]
    [SwaggerResponse(200, "Asset settings found", typeof(AssetSettingsResource))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetEffectiveAssetSettingsByAssetId(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        CancellationToken cancellationToken = default)
    {
        var result = await assetSettingsQueryService.Handle(
            new GetEffectiveAssetSettingsByAssetIdQuery(organizationId, assetId),
            cancellationToken);
        return ActionResultFromGetEffectiveAssetSettingsByAssetResultAssembler
            .ToActionResultFromGetEffectiveAssetSettingsByAssetResult(result, this, localizer);
    }

    [HttpPut("api/v1/organizations/{organizationId:int}/assets/{assetId:int}/settings")]
    [SwaggerOperation(
        Summary = "Saves settings for an asset",
        Description = "Creates or updates settings for a specific asset",
        OperationId = "SaveAssetSettings")]
    [SwaggerResponse(200, "Asset settings saved", typeof(AssetSettingsResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SaveAssetSettings(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        [FromBody] SaveAssetSettingsResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SaveAssetSettingsCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId, assetId);
            var result = await assetSettingsCommandService.Handle(command, cancellationToken);
            return ActionResultFromSaveAssetSettingsResultAssembler
                .ToActionResultFromSaveAssetSettingsResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid asset settings request for organization {OrganizationId} asset {AssetId}",
                organizationId, assetId);
            return BadRequest(localizer["InvalidAssetSettingsRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error saving asset settings for organization {OrganizationId} asset {AssetId}",
                organizationId, assetId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorSavingAssetSettings"].Value,
                statusCode: 500);
        }
    }

    [HttpPut("api/v1/organizations/{organizationId:int}/asset-settings/default")]
    [SwaggerOperation(
        Summary = "Saves organization default asset settings",
        Description = "Creates or updates the organization-wide default asset settings",
        OperationId = "SaveDefaultAssetSettings")]
    [SwaggerResponse(200, "Default asset settings saved", typeof(AssetSettingsResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SaveDefaultAssetSettings(
        [FromRoute] int organizationId,
        [FromBody] SaveAssetSettingsResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SaveAssetSettingsCommandFromResourceAssembler
                .ToCommandFromResource(resource, organizationId);
            var result = await assetSettingsCommandService.Handle(command, cancellationToken);
            return ActionResultFromSaveAssetSettingsResultAssembler
                .ToActionResultFromSaveAssetSettingsResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid default asset settings request for organization {OrganizationId}",
                organizationId);
            return BadRequest(localizer["InvalidAssetSettingsRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error saving default asset settings for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorSavingAssetSettings"].Value,
                statusCode: 500);
        }
    }
}