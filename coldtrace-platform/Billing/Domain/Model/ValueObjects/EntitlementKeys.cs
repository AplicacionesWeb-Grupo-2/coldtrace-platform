namespace ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Stable entitlement keys consumed by backend guards and clients.
/// </summary>
public static class EntitlementKeys
{
    public const string Locations = "locations";
    public const string Assets = "assets";
    public const string IotDevices = "iot-devices";
    public const string Users = "users";
    public const string ReportHistory = "report-history";
    public const string Exports = "exports";
    public const string Maintenance = "maintenance";
    public const string AiGuidance = "ai-guidance";
    public const string AiReportSummary = "ai-report-summary";
}
