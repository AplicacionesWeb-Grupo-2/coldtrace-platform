using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Iam.Domain.Repositories;

/// <summary>
///     Password reset request repository contract.
/// </summary>
public interface IPasswordResetRequestRepository : IBaseRepository<PasswordResetRequest>
{
    /// <summary>
    ///     Finds reset metadata by its token hash.
    /// </summary>
    Task<PasswordResetRequest?> FindByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);
}
