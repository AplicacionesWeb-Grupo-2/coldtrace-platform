namespace ColdTrace.Platform.AiAssistance.Domain.Model.Commands;

/// <summary>
///     Command for generating an advisory dashboard interpretation from organization-owned data.
/// </summary>
public record GenerateDashboardAiInterpretationCommand
{
    private const int QuestionMaxLength = 240;
    private const int LanguagePreferenceMaxLength = 128;

    public GenerateDashboardAiInterpretationCommand(
        int organizationId,
        string? question,
        string? preferredLanguage,
        string? acceptLanguageHeader)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));

        OrganizationId = organizationId;
        Question = NormalizeQuestion(question);
        PreferredLanguage = NormalizeLanguagePreference(preferredLanguage);
        AcceptLanguageHeader = NormalizeLanguagePreference(acceptLanguageHeader);
    }

    public int OrganizationId { get; }

    public string? Question { get; }

    public string? PreferredLanguage { get; }

    public string? AcceptLanguageHeader { get; }

    private static string? NormalizeQuestion(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var normalized = value.Trim();
        if (normalized.Length > QuestionMaxLength)
            throw new ArgumentException(
                $"Question cannot exceed {QuestionMaxLength} characters.",
                nameof(value));
        return normalized;
    }

    private static string? NormalizeLanguagePreference(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var normalized = value.Trim();
        return normalized.Length <= LanguagePreferenceMaxLength
            ? normalized
            : normalized[..LanguagePreferenceMaxLength];
    }
}
