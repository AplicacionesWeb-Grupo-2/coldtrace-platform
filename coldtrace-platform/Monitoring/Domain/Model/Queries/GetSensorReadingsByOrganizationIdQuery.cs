namespace ColdTrace.Platform.Monitoring.Domain.Model.Queries;

/// <summary>
///     Query for getting sensor readings by organization with optional filters.
/// </summary>
public record GetSensorReadingsByOrganizationIdQuery(
    int OrganizationId,
    int? AssetId,
    int? IotDeviceId,
    DateTimeOffset? From,
    DateTimeOffset? To);
