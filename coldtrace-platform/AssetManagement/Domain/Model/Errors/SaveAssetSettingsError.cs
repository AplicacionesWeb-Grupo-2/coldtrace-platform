namespace ColdTrace.Platform.AssetManagement.Domain.Model.Errors;

/// <summary>
///     Errors produced while saving asset settings.
/// </summary>
public enum SaveAssetSettingsError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
