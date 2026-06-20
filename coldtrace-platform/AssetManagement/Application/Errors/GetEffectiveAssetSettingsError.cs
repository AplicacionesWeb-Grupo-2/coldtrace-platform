namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors produced while resolving effective asset settings.
/// </summary>
public enum GetEffectiveAssetSettingsError
{
    AssetNotFound,
    AssetSettingsNotFound,
    UnexpectedError
}
