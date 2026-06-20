namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Asset type entry owned by asset settings.
/// </summary>
public class AssetSettingsAssetType
{
    protected AssetSettingsAssetType()
    {
        AssetType = string.Empty;
    }

    public AssetSettingsAssetType(string assetType)
    {
        AssetType = assetType;
    }

    public string AssetType { get; private set; }
}
