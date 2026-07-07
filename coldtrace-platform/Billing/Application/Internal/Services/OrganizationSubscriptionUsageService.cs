using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Application.Internal.Services;

/// <summary>
///     Builds organization usage snapshots for entitlement evaluation.
/// </summary>
public class OrganizationSubscriptionUsageService(
    ILocationRepository locationRepository,
    IAssetRepository assetRepository,
    IIotDeviceRepository iotDeviceRepository,
    IUserRepository userRepository)
{
    public async Task<OrganizationSubscriptionUsage> SnapshotForAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken);
        var assets = await assetRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken);
        var devices = await iotDeviceRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken);
        var users = await userRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken);

        return new OrganizationSubscriptionUsage(
            locations.Count(),
            assets.Count(),
            devices.Count(),
            users.Count());
    }
}
