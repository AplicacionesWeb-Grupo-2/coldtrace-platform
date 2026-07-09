using ColdTrace.Platform.AiAssistance.Domain.Model.Commands;
using ColdTrace.Platform.AiAssistance.Interfaces.REST.Resources;

namespace ColdTrace.Platform.AiAssistance.Interfaces.REST.Transform;

/// <summary>
///     Assembles dashboard AI interpretation commands from REST input.
/// </summary>
public static class GenerateDashboardAiInterpretationCommandFromResourceAssembler
{
    public static GenerateDashboardAiInterpretationCommand ToCommandFromResource(
        GenerateDashboardAiInterpretationResource? resource,
        int organizationId,
        string? acceptLanguageHeader) =>
        new(
            organizationId,
            resource?.Question,
            resource?.PreferredLanguage,
            acceptLanguageHeader);
}
