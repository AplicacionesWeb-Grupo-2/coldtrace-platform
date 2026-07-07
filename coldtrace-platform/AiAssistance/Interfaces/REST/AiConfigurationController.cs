using System.Net.Mime;
using ColdTrace.Platform.AiAssistance.Domain.Model.Queries;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;
using ColdTrace.Platform.AiAssistance.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AiAssistance.Interfaces.REST;

/// <summary>
///     REST controller exposing AI assistance foundation diagnostics.
/// </summary>
[ApiController]
[Route("api/v1/ai-assistance")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("AI Assistance")]
public class AiConfigurationController(
    IAiProviderStatusQueryService aiProviderStatusQueryService,
    IStringLocalizer<SharedResource> localizer)
    : ControllerBase
{
    /// <summary>
    ///     Gets safe AI assistance provider configuration status.
    /// </summary>
    [HttpGet("provider-status")]
    [SwaggerOperation(
        Summary = "Gets AI assistance provider status",
        Description = "Gets non-secret AI assistance provider configuration and structured output contract status",
        OperationId = "GetAiAssistanceProviderStatus")]
    [SwaggerResponse(200, "AI assistance provider status found", typeof(AiProviderStatusResource))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetProviderStatus(CancellationToken cancellationToken = default)
    {
        var result = await aiProviderStatusQueryService.Handle(
            new GetAiProviderStatusQuery(),
            cancellationToken);

        return ActionResultFromGetAiProviderStatusResultAssembler
            .ToActionResultFromGetAiProviderStatusResult(result, this, localizer);
    }
}
