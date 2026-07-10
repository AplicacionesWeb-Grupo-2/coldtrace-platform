using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Iam.Application.Internal.OutboundServices.Social;

/// <summary>
///     Verified identity returned by an external social provider.
/// </summary>
public sealed record ProviderIdentity(
    SocialProvider Provider,
    string Subject,
    string? Email,
    string? FullName,
    string? GivenName,
    string? FamilyName,
    string IdToken);
