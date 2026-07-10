using ColdTrace.Platform.AiAssistance.Domain.Model.Commands;
using ColdTrace.Platform.AiAssistance.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.AiAssistance.Interfaces.Rest.Transform;

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
