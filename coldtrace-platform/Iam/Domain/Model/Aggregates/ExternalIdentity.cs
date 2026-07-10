using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     External provider identity linked to a local ColdTrace user.
/// </summary>
public class ExternalIdentity : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected ExternalIdentity()
    {
        ProviderSubject = string.Empty;
    }

    /// <summary>
    ///     Creates a validated provider identity link.
    /// </summary>
    public ExternalIdentity(SocialProvider provider, string providerSubject, string? email, int userId)
    {
        Provider = provider;
        ProviderSubject = RequireNonBlank(providerSubject, nameof(providerSubject));
        Email = NormalizeOptionalEmail(email);
        UserId = RequirePositive(userId, nameof(userId));
    }

    public int Id { get; private set; }

    public SocialProvider Provider { get; private set; }

    public string ProviderSubject { get; private set; }

    public string? Email { get; private set; }

    public int UserId { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    private static string RequireNonBlank(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.");
        return value.Trim();
    }

    private static string? NormalizeOptionalEmail(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();

    private static int RequirePositive(int value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} must be positive.");
        return value;
    }
}
