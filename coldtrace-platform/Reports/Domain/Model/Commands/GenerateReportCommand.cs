namespace ColdTrace.Platform.Reports.Domain.Model.Commands;

/// <summary>
///     Command for generating an organization report.
/// </summary>
public record GenerateReportCommand
{
    public GenerateReportCommand(
        int organizationId,
        string type,
        string title,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd)
    {
        OrganizationId = RequirePositive(organizationId, nameof(organizationId));
        Type = RequireNonBlank(type).ToUpperInvariant();
        Title = RequireNonBlank(title);
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;

        if (PeriodStart > PeriodEnd)
            throw new ArgumentException("Report period start cannot be after period end.");
    }

    public int OrganizationId { get; init; }

    public string Type { get; init; }

    public string Title { get; init; }

    public DateTimeOffset PeriodStart { get; init; }

    public DateTimeOffset PeriodEnd { get; init; }

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }
}
