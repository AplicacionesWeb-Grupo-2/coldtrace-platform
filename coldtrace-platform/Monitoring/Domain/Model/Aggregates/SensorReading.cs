using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;

/// <summary>
///     Sensor reading aggregate for the monitoring context.
/// </summary>
public class SensorReading : IAuditableEntity
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected SensorReading()
    {
        RecordedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Creates a sensor reading from a create command.
    /// </summary>
    /// <param name="command">Command containing reading data.</param>
    public SensorReading(CreateSensorReadingCommand command)
    {
        OrganizationId = command.OrganizationId;
        AssetId = command.AssetId;
        IotDeviceId = command.IotDeviceId;
        Temperature = command.Temperature;
        Humidity = command.Humidity;
        RecordedAt = command.RecordedAt;
        MotionDetected = command.MotionDetected;
        ImageCaptured = command.ImageCaptured;
        BatteryLevel = command.BatteryLevel;
        SignalStrength = command.SignalStrength;
    }

    /// <summary>
    ///     Creates an enriched sensor reading from validated monitoring context.
    /// </summary>
    public SensorReading(
        int organizationId,
        int assetId,
        int iotDeviceId,
        int gatewayId,
        int locationId,
        double? temperature,
        double? humidity,
        bool outOfRange,
        DateTimeOffset recordedAt,
        bool? motionDetected,
        bool? imageCaptured,
        int? batteryLevel,
        int? signalStrength)
    {
        OrganizationId = organizationId;
        AssetId = assetId;
        IotDeviceId = iotDeviceId;
        GatewayId = gatewayId;
        LocationId = locationId;
        Temperature = temperature;
        Humidity = humidity;
        OutOfRange = outOfRange;
        RecordedAt = recordedAt;
        MotionDetected = motionDetected;
        ImageCaptured = imageCaptured;
        BatteryLevel = batteryLevel;
        SignalStrength = signalStrength;
    }

    /// <summary>
    ///     Gets the server-generated reading identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the monitored asset identifier.
    /// </summary>
    public int AssetId { get; private set; }

    /// <summary>
    ///     Gets the originating IoT device identifier.
    /// </summary>
    public int IotDeviceId { get; private set; }

    /// <summary>
    ///     Gets the gateway used by the reading.
    /// </summary>
    public int GatewayId { get; private set; }

    /// <summary>
    ///     Gets the location context captured from the asset.
    /// </summary>
    public int LocationId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the originating IoT device.
    /// </summary>
    public IotDevice IotDevice { get; private set; } = null!;

    /// <summary>
    ///     Gets the monitored asset.
    /// </summary>
    public Asset Asset { get; private set; } = null!;

    /// <summary>
    ///     Gets the gateway.
    /// </summary>
    public Gateway Gateway { get; private set; } = null!;

    /// <summary>
    ///     Gets the optional temperature reading.
    /// </summary>
    public double? Temperature { get; private set; }

    /// <summary>
    ///     Gets the optional humidity reading.
    /// </summary>
    public double? Humidity { get; private set; }

    /// <summary>
    ///     Gets whether the reading is outside configured thresholds.
    /// </summary>
    public bool OutOfRange { get; private set; }

    /// <summary>
    ///     Gets the timestamp when the reading was captured.
    /// </summary>
    public DateTimeOffset RecordedAt { get; private set; }

    /// <summary>
    ///     Gets whether motion was detected.
    /// </summary>
    public bool? MotionDetected { get; private set; }

    /// <summary>
    ///     Gets whether an image was captured.
    /// </summary>
    public bool? ImageCaptured { get; private set; }

    /// <summary>
    ///     Gets the optional battery percentage.
    /// </summary>
    public int? BatteryLevel { get; private set; }

    /// <summary>
    ///     Gets the optional signal strength percentage.
    /// </summary>
    public int? SignalStrength { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
