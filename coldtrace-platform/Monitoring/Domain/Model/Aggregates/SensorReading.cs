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
        Metric = string.Empty;
        Unit = string.Empty;
        RecordedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Creates a sensor reading from a create command.
    /// </summary>
    /// <param name="command">Command containing reading data.</param>
    public SensorReading(CreateSensorReadingCommand command)
    {
        OrganizationId = command.OrganizationId;
        IotDeviceId = command.IotDeviceId;
        Metric = command.Metric;
        Value = command.Value;
        Unit = command.Unit;
        RecordedAt = command.RecordedAt;
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
    ///     Gets the originating IoT device identifier.
    /// </summary>
    public int IotDeviceId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the originating IoT device.
    /// </summary>
    public IotDevice IotDevice { get; private set; } = null!;

    /// <summary>
    ///     Gets the metric name.
    /// </summary>
    public string Metric { get; private set; }

    /// <summary>
    ///     Gets the measured value.
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    ///     Gets the metric unit.
    /// </summary>
    public string Unit { get; private set; }

    /// <summary>
    ///     Gets the timestamp when the reading was captured.
    /// </summary>
    public DateTimeOffset RecordedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
