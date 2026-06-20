using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Asset command application service contract.
/// </summary>
public interface IAssetCommandService
{
    Task<Result<Asset, CreateAssetError>> Handle(
        CreateAssetCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Asset, UpdateAssetError>> Handle(
        UpdateAssetCommand command,
        CancellationToken cancellationToken = default);
}