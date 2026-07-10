using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Iam.Domain.Repositories;

/// <summary>
///     Organization repository contract.
/// </summary>
public interface IOrganizationRepository : IBaseRepository<Organization>
{
    /// <summary>
    ///     Determines whether an organization exists with the provided contact email.
    /// </summary>
    /// <param name="contactEmail">Organization contact email.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when an organization with the same contact email exists.</returns>
    Task<bool> ExistsByContactEmailAsync(string contactEmail, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether an organization exists with the provided tax identifier.
    /// </summary>
    /// <param name="taxId">Organization tax identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when an organization with the same tax identifier exists.</returns>
    Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);
}
