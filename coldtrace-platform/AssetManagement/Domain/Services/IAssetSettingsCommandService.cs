using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Asset settings command application service contract.
/// </summary>
public interface IAssetSettingsCommandService
{
    Task<Result<AssetSettings, SaveAssetSettingsError>> Handle(
        SaveAssetSettingsCommand command,
        CancellationToken cancellationToken = default);
}