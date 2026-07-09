using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;

/// <summary>
///     Optional request payload for dashboard AI interpretation generation.
/// </summary>
[SwaggerSchema(Description = "Optional request payload for dashboard AI interpretation")]
public record GenerateDashboardAiInterpretationResource(
    [StringLength(240)]
    [SwaggerParameter(Description = "Optional operator question")]
    string? Question,
    [StringLength(32)]
    [SwaggerParameter(Description = "Optional response language preference, such as es or en")]
    string? PreferredLanguage);
