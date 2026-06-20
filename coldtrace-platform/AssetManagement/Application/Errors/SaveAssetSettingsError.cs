namespace ColdTrace.Platform.AssetManagement.Application.Errors;

/// <summary>
///     Errors produced while saving asset settings.
/// </summary>
public enum SaveAssetSettingsError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
