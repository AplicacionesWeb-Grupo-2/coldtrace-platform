using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Interfaces.Rest.Resources;
using ColdTrace.Platform.AiAssistance.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an AI provider status result.
/// </summary>
public static class ActionResultFromGetAiProviderStatusResultAssembler
{
    public static ActionResult ToActionResultFromGetAiProviderStatusResult(
        Result<AiProviderStatus, GetAiProviderStatusError> result,
        ControllerBase controller,
        IStringLocalizer<AiAssistanceMessages> localizer) =>
        result switch
        {
            Result<AiProviderStatus, GetAiProviderStatusError>.Success success =>
                controller.Ok(AiProviderStatusResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<AiProviderStatus, GetAiProviderStatusError>.Failure =>
                controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError),

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
