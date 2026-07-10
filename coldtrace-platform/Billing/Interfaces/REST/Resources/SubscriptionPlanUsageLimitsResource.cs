using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing subscription plan usage limits.
/// </summary>
[SwaggerSchema(Description = "Subscription plan usage limits")]
public record SubscriptionPlanUsageLimitsResource(
    [SwaggerParameter(Description = "Maximum locations allowed")]
    int? MaxLocations,
    [SwaggerParameter(Description = "Maximum assets allowed")]
    int? MaxAssets,
    [SwaggerParameter(Description = "Maximum IoT devices allowed")]
    int? MaxIotDevices,
    [SwaggerParameter(Description = "Maximum users allowed")]
    int? MaxUsers,
    [SwaggerParameter(Description = "Report and telemetry history retention window")]
    int? HistoryRetentionDays);
