using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Domain.Repositories;

/// <summary>
///     Repository contract for external identity links.
/// </summary>
public interface IExternalIdentityRepository : IBaseRepository<ExternalIdentity>
{
    Task<ExternalIdentity?> FindByProviderAndSubjectAsync(
        SocialProvider provider,
        string providerSubject,
        CancellationToken cancellationToken = default);
}
