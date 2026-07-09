using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices.Social;

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
