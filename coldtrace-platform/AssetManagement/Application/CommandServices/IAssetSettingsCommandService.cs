using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.CommandServices;

/// <summary>
///     Application service contract for asset settings commands.
/// </summary>
public interface IAssetSettingsCommandService
{
    Task<Result<AssetSettings, SaveAssetSettingsError>> Handle(
        SaveAssetSettingsCommand command,
        CancellationToken cancellationToken = default);
}
