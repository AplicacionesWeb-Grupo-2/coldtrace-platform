using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AiAssistance.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI provider status result.
/// </summary>
public static class ActionResultFromGetAiProviderStatusResultAssembler
{
    public static ActionResult ToActionResultFromGetAiProviderStatusResult(
        Result<AiProviderStatus, GetAiProviderStatusError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AiProviderStatus, GetAiProviderStatusError>.Success success =>
                controller.Ok(AiProviderStatusResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<AiProviderStatus, GetAiProviderStatusError>.Failure =>
                controller.Problem(
                    title: localizer["UnexpectedServerError"].Value,
                    detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                    statusCode: 500),

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
