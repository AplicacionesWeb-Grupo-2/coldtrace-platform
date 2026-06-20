namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     IoT device type entry owned by asset settings.
/// </summary>
public class AssetSettingsIotDeviceType
{
    protected AssetSettingsIotDeviceType()
    {
        IotDeviceType = string.Empty;
    }

    public AssetSettingsIotDeviceType(string iotDeviceType)
    {
        IotDeviceType = iotDeviceType;
    }

    public string IotDeviceType { get; private set; }
}
