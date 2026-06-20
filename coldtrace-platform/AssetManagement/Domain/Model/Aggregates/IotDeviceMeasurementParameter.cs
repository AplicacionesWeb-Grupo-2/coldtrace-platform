namespace ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;

/// <summary>
///     Measurement parameter entry owned by an IoT device.
/// </summary>
public class IotDeviceMeasurementParameter
{
    protected IotDeviceMeasurementParameter()
    {
        MeasurementParameter = string.Empty;
    }

    public IotDeviceMeasurementParameter(string measurementParameter)
    {
        MeasurementParameter = measurementParameter;
    }

    public string MeasurementParameter { get; private set; }
}
