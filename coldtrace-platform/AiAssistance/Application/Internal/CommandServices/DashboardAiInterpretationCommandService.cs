using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using ColdTrace.Platform.AiAssistance.Domain.Model.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.Results;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Domain.Model.Commands;
using ColdTrace.Platform.AiAssistance.Application.CommandServices;
using ColdTrace.Platform.AiAssistance.Application.QueryServices;
using ColdTrace.Platform.AiAssistance.Application.Internal.OutboundServices;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace ColdTrace.Platform.AiAssistance.Application.Internal.CommandServices;

/// <summary>
///     Generates dashboard interpretations from organization-owned persisted evidence.
/// </summary>
public class DashboardAiInterpretationCommandService(
    IIamContextFacade iamContextFacade,
    ISensorReadingRepository sensorReadingRepository,
    IIncidentRepository incidentRepository,
    IAssetRepository assetRepository,
    IIotDeviceRepository iotDeviceRepository,
    IGatewayRepository gatewayRepository,
    IReportRepository reportRepository,
    IMaintenanceScheduleRepository maintenanceScheduleRepository,
    ITechnicalServiceRequestRepository technicalServiceRequestRepository,
    IAiStructuredOutputService aiStructuredOutputService,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IOptions<AiOptions> aiOptions,
    ILogger<DashboardAiInterpretationCommandService> logger)
    : IDashboardAiInterpretationCommandService
{
    private const int MaxRecentReadings = 16;
    private const int MaxRecentIncidents = 12;
    private const int MaxAssetEvidence = 10;
    private const int MaxDeviceEvidence = 10;
    private const int MaxGatewayEvidence = 10;
    private const int MaxRecentReports = 5;
    private const int MaxMaintenanceRecords = 8;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static readonly char[] SpanishMarkers = ['á', 'é', 'í', 'ó', 'ú', 'ñ', 'ü', '¿', '¡'];

    private static readonly string[] SpanishTokens =
    [
        "actualmente", "cual", "cuales", "cuál", "cuáles", "cuando", "cuándo", "cuanto", "cuánto",
        "detectada", "detectadas", "detectado", "detectados", "donde", "dónde", "fueron", "incidencia",
        "incidencias", "primero", "que", "qué", "revisar", "riesgo", "riesgos", "ultimas", "últimas"
    ];

    public async Task<Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>> Handle(
        GenerateDashboardAiInterpretationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning(
                "Organization not found for dashboard AI interpretation: {OrganizationId}",
                command.OrganizationId);
            return Failure(GenerateDashboardAiInterpretationError.OrganizationNotFound);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementAiGuidance,
            "DashboardAiInterpretationPlanLimitExceeded",
            cancellationToken);

        try
        {
            var responseLanguage = ResolveResponseLanguage(command);
            var context = await BuildDashboardInterpretationContextAsync(
                command.OrganizationId,
                command.Question,
                responseLanguage,
                cancellationToken);
            var serializedContext = JsonSerializer.Serialize(context, SerializerOptions);
            var generationResult = await aiStructuredOutputService.GenerateDashboardInterpretationAsync(
                BuildPrompt(serializedContext, responseLanguage),
                cancellationToken);

            if (generationResult is Result<DashboardInterpretationOutput, AiGenerationError>.Failure aiFailure)
                return Failure(MapAiGenerationError(aiFailure.Error));

            var output = ((Result<DashboardInterpretationOutput, AiGenerationError>.Success)generationResult).Value;
            var interpretation = LocalizeDashboardInterpretation(
                SanitizeDashboardInterpretation(output, context),
                responseLanguage);
            var options = aiOptions.Value;

            logger.LogInformation(
                "AI dashboard interpretation generated: {OrganizationId} {ModelProvider} {ModelName}",
                command.OrganizationId,
                options.Provider,
                options.Model);

            return new Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>.Success(
                new DashboardAiInterpretation(
                    command.OrganizationId,
                    command.Question,
                    interpretation,
                    context.SourceMetrics,
                    options.Provider,
                    options.Model ?? "not-configured",
                    DateTimeOffset.UtcNow));
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Dashboard context serialization failed for AI interpretation");
            return Failure(GenerateDashboardAiInterpretationError.DashboardContextUnavailable);
        }
        catch (NotSupportedException ex)
        {
            logger.LogWarning(ex, "Dashboard context could not be serialized for AI interpretation");
            return Failure(GenerateDashboardAiInterpretationError.DashboardContextUnavailable);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error generating dashboard AI interpretation for organization {OrganizationId}",
                command.OrganizationId);
            return Failure(GenerateDashboardAiInterpretationError.UnexpectedError);
        }
    }

    private async Task<DashboardInterpretationContext> BuildDashboardInterpretationContextAsync(
        int organizationId,
        string? question,
        string responseLanguage,
        CancellationToken cancellationToken)
    {
        var readings = (await sensorReadingRepository.FindAllByOrganizationIdAsync(
                organizationId,
                cancellationToken: cancellationToken))
            .OrderByDescending(reading => reading.RecordedAt)
            .ThenByDescending(reading => reading.Id)
            .ToList();
        var incidents = (await incidentRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .OrderByDescending(incident => incident.DetectedAt)
            .ToList();
        var reports = (await reportRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .OrderByDescending(report => report.GeneratedAt)
            .Take(MaxRecentReports)
            .ToList();
        var allAssets = (await assetRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .OrderBy(asset => asset.Id)
            .ToList();

        var referencedAssetIds = readings
            .Select(reading => reading.AssetId)
            .Concat(incidents.Select(incident => incident.AssetId).OfType<int>())
            .ToHashSet();
        var assets = allAssets
            .Where(asset => referencedAssetIds.Contains(asset.Id))
            .ToList();
        var loadedAssetIds = assets.Select(asset => asset.Id).ToHashSet();

        var referencedDeviceIds = readings.Select(reading => reading.IotDeviceId).ToHashSet();
        var devices = (await iotDeviceRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .Where(device => referencedDeviceIds.Contains(device.Id))
            .OrderBy(device => device.Id)
            .ToList();

        var referencedGatewayIds = readings
            .Select(reading => reading.GatewayId)
            .Concat(devices.Select(device => device.GatewayId))
            .ToHashSet();
        var gateways = (await gatewayRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .Where(gateway => referencedGatewayIds.Contains(gateway.Id))
            .OrderBy(gateway => gateway.Id)
            .ToList();

        var maintenanceSchedules = (await maintenanceScheduleRepository.FindAllByOrganizationIdAsync(
                organizationId,
                cancellationToken))
            .Where(schedule => loadedAssetIds.Contains(schedule.AssetId))
            .OrderBy(schedule => schedule.ScheduledDate)
            .ToList();
        var incidentIds = incidents.Select(incident => incident.Id).ToHashSet();
        var technicalServiceRequests = (await technicalServiceRequestRepository.FindAllByOrganizationIdAsync(
                organizationId,
                cancellationToken))
            .Where(request => loadedAssetIds.Contains(request.AssetId) ||
                              request.IncidentId is int incidentId && incidentIds.Contains(incidentId))
            .OrderByDescending(request => request.RequestedAt)
            .ToList();

        var metrics = BuildMetrics(
            allAssets.Count,
            readings,
            incidents,
            reports,
            maintenanceSchedules,
            technicalServiceRequests,
            devices,
            gateways);
        var sourceMetrics = ToSourceMetrics(metrics);

        return new DashboardInterpretationContext(
            "dashboard-ai-interpretation.v1",
            organizationId,
            question,
            responseLanguage,
            metrics,
            sourceMetrics,
            readings.Take(MaxRecentReadings).Select(ToReadingEvidenceContext).ToList(),
            incidents.Take(MaxRecentIncidents).Select(ToIncidentEvidenceContext).ToList(),
            assets.Take(MaxAssetEvidence).Select(ToAssetEvidenceContext).ToList(),
            devices.Take(MaxDeviceEvidence).Select(ToDeviceEvidenceContext).ToList(),
            gateways.Take(MaxGatewayEvidence).Select(ToGatewayEvidenceContext).ToList(),
            reports.Select(ToReportEvidenceContext).ToList(),
            new MaintenanceEvidenceContext(
                maintenanceSchedules.Take(MaxMaintenanceRecords).Select(ToMaintenanceScheduleEvidenceContext)
                    .ToList(),
                technicalServiceRequests.Take(MaxMaintenanceRecords)
                    .Select(ToTechnicalServiceRequestEvidenceContext)
                    .ToList()),
            BuildEvidenceNotes(
                readings,
                incidents,
                assets,
                devices,
                gateways,
                reports,
                maintenanceSchedules,
                technicalServiceRequests));
    }

    private static DashboardMetricContext BuildMetrics(
        int assetCount,
        IReadOnlyCollection<SensorReading> readings,
        IReadOnlyCollection<Incident> incidents,
        IReadOnlyCollection<Report> reports,
        IReadOnlyCollection<MaintenanceSchedule> maintenanceSchedules,
        IReadOnlyCollection<TechnicalServiceRequest> technicalServiceRequests,
        IReadOnlyCollection<IotDevice> devices,
        IReadOnlyCollection<Gateway> gateways)
    {
        var readingsCount = readings.Count;
        var outOfRangeCount = readings.Count(reading => reading.OutOfRange);
        var averageTemperature = Average(readings.Select(reading => reading.Temperature));
        var averageHumidity = Average(readings.Select(reading => reading.Humidity));
        double? thermalCompliance = readingsCount == 0
            ? null
            : RoundOneDecimal((double)(readingsCount - outOfRangeCount) / readingsCount * 100);

        return new DashboardMetricContext(
            assetCount,
            readingsCount,
            outOfRangeCount,
            thermalCompliance,
            averageTemperature,
            averageHumidity,
            incidents.Count,
            incidents.Count(IsOperationallyOpen),
            reports.Count,
            maintenanceSchedules.Count,
            technicalServiceRequests.Count,
            technicalServiceRequests.Count(IsOpenServiceRequest),
            devices.Count(device => IsUnhealthyStatus(device.Status)),
            gateways.Count(gateway => IsUnhealthyStatus(gateway.Status)));
    }

    private static IReadOnlyCollection<DashboardSourceMetricOutput> ToSourceMetrics(
        DashboardMetricContext metrics) =>
    [
        new(
            "monitoredAssets",
            FormatInteger(metrics.MonitoredAssets),
            "assets",
            "Assets registered for the organization"),
        new(
            "readingsReviewed",
            FormatInteger(metrics.ReadingsReviewed),
            "readings",
            "Persisted sensor readings reviewed for the dashboard interpretation"),
        new(
            "outOfRangeReadings",
            FormatInteger(metrics.OutOfRangeReadings),
            "readings",
            "Readings flagged as outside configured safe ranges"),
        new(
            "thermalCompliance",
            FormatMetric(metrics.ThermalCompliancePercentage),
            "%",
            "Percentage of reviewed readings inside the configured safe range"),
        new(
            "averageTemperature",
            FormatMetric(metrics.AverageTemperature),
            "C",
            "Average temperature across reviewed readings"),
        new(
            "averageHumidity",
            FormatMetric(metrics.AverageHumidity),
            "%",
            "Average humidity across reviewed readings"),
        new(
            "openIncidents",
            FormatInteger(metrics.OpenIncidents),
            "incidents",
            "Incidents that are not resolved"),
        new(
            "recentReports",
            FormatInteger(metrics.RecentReports),
            "reports",
            "Most recent persisted reports available to support the dashboard interpretation"),
        new(
            "technicalServiceRequests",
            FormatInteger(metrics.TechnicalServiceRequests),
            "requests",
            "Technical service requests tied to referenced assets or incidents"),
        new(
            "openTechnicalServiceRequests",
            FormatInteger(metrics.OpenTechnicalServiceRequests),
            "requests",
            "Technical service requests that are not closed"),
        new(
            "maintenanceSchedules",
            FormatInteger(metrics.MaintenanceSchedules),
            "schedules",
            "Preventive maintenance schedules tied to referenced assets"),
        new(
            "devicesWithUnhealthyStatus",
            FormatInteger(metrics.DevicesWithUnhealthyStatus),
            "devices",
            "Referenced IoT devices whose status is not online or active"),
        new(
            "gatewaysWithUnhealthyStatus",
            FormatInteger(metrics.GatewaysWithUnhealthyStatus),
            "gateways",
            "Referenced gateways whose status is not online or active")
    ];

    private static IReadOnlyCollection<string> BuildEvidenceNotes(
        IReadOnlyCollection<SensorReading> readings,
        IReadOnlyCollection<Incident> incidents,
        IReadOnlyCollection<Asset> assets,
        IReadOnlyCollection<IotDevice> devices,
        IReadOnlyCollection<Gateway> gateways,
        IReadOnlyCollection<Report> reports,
        IReadOnlyCollection<MaintenanceSchedule> maintenanceSchedules,
        IReadOnlyCollection<TechnicalServiceRequest> technicalServiceRequests)
    {
        var notes = new List<string>();
        if (readings.Count == 0)
            notes.Add("No sensor readings were available for dashboard interpretation.");
        if (incidents.Count == 0)
            notes.Add("No incidents were available for dashboard interpretation.");
        if (readings.Count > 0 && assets.Count == 0)
            notes.Add("Asset details are limited because referenced assets could not be loaded.");
        if (readings.Count > 0 && devices.Count == 0)
            notes.Add("IoT device details are limited because referenced devices could not be loaded.");
        if (readings.Count > 0 && gateways.Count == 0)
            notes.Add("Gateway details are limited because referenced gateways could not be loaded.");
        if (reports.Count == 0)
            notes.Add("No persisted reports were available to support trend or compliance interpretation.");
        if (maintenanceSchedules.Count == 0 && technicalServiceRequests.Count == 0)
            notes.Add(
                "Maintenance progress is limited because no related schedules or technical service requests were found.");
        notes.Add("Notification details are not included because the current alerts ACL exposes notifications only through REST.");
        return notes;
    }

    private static AiStructuredPrompt BuildPrompt(string contextJson, string responseLanguage) =>
        new(
            """
            You are ColdTrace's backend AI assistant. Generate advisory operational guidance only.
            Do not claim that an incident is resolved, closed, escalated, or mutated.
            Use only the provided backend context. If context is missing, state uncertainty in uncertaintyNotes.
            Return one structured response only.
            """,
            $"""
             Interpret this ColdTrace dashboard context for an operations user.

             Response language: {responseLanguage}

             Requirements:
             - Include overallReading, attentionLevel, metricInsights, risks, recommendedActions, and uncertaintyNotes.
             - The attentionLevel must be one concise operational label such as stable, attention recommended, or critical attention.
             - Return exactly 3 metricInsights, exactly 2 risks, exactly 2 recommendedActions, and exactly 1 uncertaintyNotes item.
             - Keep overallReading under 240 characters.
             - Keep every insight title under 50 characters and every insight interpretation under 180 characters.
             - Keep every risk, recommendedAction, and uncertaintyNote under 160 characters.
             - Keep every string short and operational; use one sentence per item.
             - Write every user-facing string value in the response language above.
             - If the response language is Spanish, do not answer in English.
             - Do not invent source data that is absent from the context.
             """,
            new Dictionary<string, string> { ["dashboardContext"] = contextJson });

    private static string ResolveResponseLanguage(GenerateDashboardAiInterpretationCommand command) =>
        ResolveLanguagePreference(command.PreferredLanguage) ??
        ResolveAcceptLanguage(command.AcceptLanguageHeader) ??
        DetectResponseLanguage(command.Question);

    private static string? ResolveAcceptLanguage(string? acceptLanguageHeader)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguageHeader)) return null;
        if (!StringWithQualityHeaderValue.TryParseList([acceptLanguageHeader], out var languages)) return null;

        return languages
            .OrderByDescending(language => language.Quality ?? 1)
            .Select(language => ResolveLanguagePreference(language.Value.ToString()))
            .FirstOrDefault(language => language is not null);
    }

    private static string? ResolveLanguagePreference(string? languagePreference)
    {
        if (string.IsNullOrWhiteSpace(languagePreference)) return null;

        var normalized = languagePreference.Trim().ToLowerInvariant().Replace('_', '-');
        if (normalized is "spanish" or "espanol" or "español" || normalized.StartsWith("es", StringComparison.Ordinal))
            return "Spanish";
        if (normalized == "english" || normalized.StartsWith("en", StringComparison.Ordinal))
            return "English";
        return null;
    }

    private static string DetectResponseLanguage(string? question)
    {
        if (string.IsNullOrWhiteSpace(question)) return "English";

        var normalized = question.ToLowerInvariant();
        if (normalized.IndexOfAny(SpanishMarkers) >= 0) return "Spanish";

        var paddedQuestion = " " + Regex.Replace(normalized, "[^\\p{L}\\p{N}]+", " ") + " ";
        return SpanishTokens.Any(token => paddedQuestion.Contains($" {token} ", StringComparison.Ordinal))
            ? "Spanish"
            : "English";
    }

    private static DashboardInterpretationOutput SanitizeDashboardInterpretation(
        DashboardInterpretationOutput interpretation,
        DashboardInterpretationContext context) =>
        new(
            SanitizeText(interpretation.OverallReading, 500, FallbackOverallReading(context))!,
            SanitizeText(interpretation.AttentionLevel, 80, FallbackAttentionLevel(context))!,
            SanitizeInsights(interpretation.MetricInsights, context),
            SanitizeTextList(interpretation.Risks, FallbackRisks(context), 2, 2, 240),
            SanitizeTextList(interpretation.RecommendedActions, FallbackRecommendedActions(context), 2, 2, 240),
            SanitizeTextList(interpretation.UncertaintyNotes, FallbackUncertaintyNotes(context), 1, 1, 240));

    private static DashboardInterpretationOutput LocalizeDashboardInterpretation(
        DashboardInterpretationOutput interpretation,
        string responseLanguage) =>
        responseLanguage.Equals("Spanish", StringComparison.OrdinalIgnoreCase)
            ? interpretation with { AttentionLevel = LocalizeSpanishAttentionLevel(interpretation.AttentionLevel) }
            : interpretation;

    private static string LocalizeSpanishAttentionLevel(string attentionLevel)
    {
        if (string.IsNullOrWhiteSpace(attentionLevel)) return attentionLevel;

        var normalized = attentionLevel.Trim().ToLowerInvariant();
        if (normalized.Contains("critical", StringComparison.Ordinal)) return "atención crítica";
        if (normalized.Contains("attention", StringComparison.Ordinal) ||
            normalized.Contains("review", StringComparison.Ordinal))
            return "revisión recomendada";
        return normalized.Contains("stable", StringComparison.Ordinal) ? "estable" : attentionLevel;
    }

    private static IReadOnlyCollection<DashboardInterpretationInsightOutput> SanitizeInsights(
        IReadOnlyCollection<DashboardInterpretationInsightOutput>? insights,
        DashboardInterpretationContext context)
    {
        var sanitized = new List<DashboardInterpretationInsightOutput>();
        if (insights is not null)
        {
            foreach (var insight in insights)
            {
                var sanitizedInsight = SanitizeInsight(insight);
                if (sanitizedInsight is not null) sanitized.Add(sanitizedInsight);
                if (sanitized.Count == 3) break;
            }
        }

        foreach (var fallback in FallbackInsights(context))
        {
            if (sanitized.Count == 3) break;
            sanitized.Add(fallback);
        }

        return sanitized;
    }

    private static DashboardInterpretationInsightOutput? SanitizeInsight(
        DashboardInterpretationInsightOutput insight)
    {
        var title = SanitizeText(insight.Title, 100, null);
        var metric = SanitizeText(insight.Metric, 100, null);
        var interpretation = SanitizeText(insight.Interpretation, 360, null);
        var severity = SanitizeText(insight.Severity, 80, null);
        return title is null || metric is null || interpretation is null || severity is null
            ? null
            : new DashboardInterpretationInsightOutput(title, metric, interpretation, severity);
    }

    private static IReadOnlyCollection<string> SanitizeTextList(
        IReadOnlyCollection<string>? values,
        IReadOnlyCollection<string> fallbacks,
        int minimumSize,
        int maximumSize,
        int maximumTextLength)
    {
        var sanitized = new List<string>();
        if (values is not null)
        {
            foreach (var value in values)
            {
                var sanitizedValue = SanitizeText(value, maximumTextLength, null);
                if (sanitizedValue is not null) sanitized.Add(sanitizedValue);
                if (sanitized.Count == maximumSize) break;
            }
        }

        foreach (var fallback in fallbacks)
        {
            if (sanitized.Count >= minimumSize) break;
            var sanitizedFallback = SanitizeText(fallback, maximumTextLength, null);
            if (sanitizedFallback is not null) sanitized.Add(sanitizedFallback);
        }

        return sanitized.Take(maximumSize).ToList();
    }

    private static string? SanitizeText(string? value, int maxLength, string? fallback)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) return null;

        normalized = normalized
            .Replace("&#x0A;", " ", StringComparison.Ordinal)
            .Replace("\\n", " ", StringComparison.Ordinal);
        normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
        return LimitText(normalized, maxLength);
    }

    private static string FallbackOverallReading(DashboardInterpretationContext context) =>
        IsSpanish(context.ResponseLanguage)
            ? "El dashboard requiere revisión operativa con base en las métricas persistidas de ColdTrace."
            : "The dashboard requires operational review using persisted ColdTrace metrics.";

    private static string FallbackAttentionLevel(DashboardInterpretationContext context) =>
        context.Metrics.OpenIncidents > 0 || context.Metrics.OutOfRangeReadings > 0
            ? "critical attention"
            : "stable";

    private static IReadOnlyCollection<DashboardInterpretationInsightOutput> FallbackInsights(
        DashboardInterpretationContext context)
    {
        var metrics = context.Metrics;
        if (IsSpanish(context.ResponseLanguage))
            return
            [
                new(
                    "Cumplimiento térmico",
                    "thermalCompliancePercentage",
                    "El cumplimiento térmico se calcula desde las lecturas persistidas del dashboard.",
                    FallbackSeverity(metrics.ThermalCompliancePercentage)),
                new(
                    "Presión de incidencias",
                    "openIncidents",
                    $"El dashboard registra {metrics.OpenIncidents} incidencia(s) abierta(s).",
                    metrics.OpenIncidents > 0 ? "atención" : "estable"),
                new(
                    "Seguimiento de mantenimiento",
                    "openTechnicalServiceRequests",
                    "Las solicitudes técnicas abiertas ayudan a priorizar acciones correctivas.",
                    metrics.OpenTechnicalServiceRequests > 0 ? "media" : "estable")
            ];

        return
        [
            new(
                "Thermal compliance",
                "thermalCompliancePercentage",
                "Thermal compliance is calculated from persisted dashboard readings.",
                FallbackSeverity(metrics.ThermalCompliancePercentage)),
            new(
                "Incident pressure",
                "openIncidents",
                $"The dashboard records {metrics.OpenIncidents} open incident(s).",
                metrics.OpenIncidents > 0 ? "attention" : "stable"),
            new(
                "Maintenance follow-up",
                "openTechnicalServiceRequests",
                "Open technical service requests help prioritize corrective actions.",
                metrics.OpenTechnicalServiceRequests > 0 ? "medium" : "stable")
        ];
    }

    private static string FallbackSeverity(double? compliancePercentage) =>
        compliancePercentage is null
            ? "limited evidence"
            : compliancePercentage >= 95.0
                ? "stable"
                : "attention";

    private static IReadOnlyCollection<string> FallbackRisks(DashboardInterpretationContext context) =>
        IsSpanish(context.ResponseLanguage)
            ?
            [
                "Las incidencias abiertas pueden mantener riesgo operativo hasta su cierre manual.",
                "Las lecturas fuera de rango requieren validación antes de confirmar cumplimiento."
            ]
            :
            [
                "Open incidents can keep operational risk active until manual closure.",
                "Out-of-range readings require validation before confirming compliance."
            ];

    private static IReadOnlyCollection<string> FallbackRecommendedActions(DashboardInterpretationContext context) =>
        IsSpanish(context.ResponseLanguage)
            ?
            [
                "Revisar primero las incidencias críticas y sus activos asociados.",
                "Confirmar evidencia correctiva antes de cerrar alertas o reportes."
            ]
            :
            [
                "Review critical incidents and their associated assets first.",
                "Confirm corrective evidence before closing alerts or reports."
            ];

    private static IReadOnlyCollection<string> FallbackUncertaintyNotes(DashboardInterpretationContext context) =>
        IsSpanish(context.ResponseLanguage)
            ? ["La interpretación es advisory y depende de la completitud de los registros persistidos."]
            : ["The interpretation is advisory and depends on the completeness of persisted records."];

    private static bool IsSpanish(string responseLanguage) =>
        responseLanguage.Equals("Spanish", StringComparison.OrdinalIgnoreCase);

    private static bool IsOperationallyOpen(Incident incident) =>
        !incident.Status.Equals(Incident.StatusResolved, StringComparison.OrdinalIgnoreCase);

    private static bool IsOpenServiceRequest(TechnicalServiceRequest request) =>
        !request.Status.Equals("closed", StringComparison.OrdinalIgnoreCase);

    private static bool IsUnhealthyStatus(string? status) =>
        status is not null &&
        !status.Equals("online", StringComparison.OrdinalIgnoreCase) &&
        !status.Equals("active", StringComparison.OrdinalIgnoreCase);

    private static double? Average(IEnumerable<double?> values)
    {
        var availableValues = values.OfType<double>().ToList();
        return availableValues.Count == 0 ? null : RoundOneDecimal(availableValues.Average());
    }

    private static double RoundOneDecimal(double value) =>
        Math.Round(value, 1, MidpointRounding.AwayFromZero);

    private static string FormatInteger(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string FormatMetric(double? value) =>
        value?.ToString("0.0", CultureInfo.InvariantCulture) ?? "unavailable";

    private static ReadingEvidenceContext ToReadingEvidenceContext(SensorReading reading) =>
        new(
            reading.Id,
            reading.AssetId,
            reading.IotDeviceId,
            reading.GatewayId,
            reading.LocationId,
            reading.Temperature,
            reading.Humidity,
            reading.OutOfRange,
            reading.RecordedAt);

    private static IncidentEvidenceContext ToIncidentEvidenceContext(Incident incident) =>
        new(
            incident.Id,
            incident.AssetId,
            LimitText(incident.Status, 80),
            incident.DetectedAt,
            IsOperationallyOpen(incident));

    private static AssetEvidenceContext ToAssetEvidenceContext(Asset asset) =>
        new(asset.Id, asset.LocationId, LimitText(asset.Name, 120));

    private static DeviceEvidenceContext ToDeviceEvidenceContext(IotDevice device) =>
        new(
            device.Id,
            device.GatewayId,
            device.AssetId,
            LimitText(device.Name, 120),
            LimitText(device.Status, 80),
            device.MeasurementParameters.ToList());

    private static GatewayEvidenceContext ToGatewayEvidenceContext(Gateway gateway) =>
        new(gateway.Id, gateway.LocationId, LimitText(gateway.Status, 80));

    private static ReportEvidenceContext ToReportEvidenceContext(Report report) =>
        new(
            report.Id,
            LimitText(report.Uuid, 80),
            LimitText(report.Type, 80),
            LimitText(report.Title, 160),
            report.GeneratedAt,
            report.ReadingCount,
            report.OutOfRangeReadingCount,
            report.OpenIncidentCount,
            report.CompliancePercentage);

    private static MaintenanceScheduleEvidenceContext ToMaintenanceScheduleEvidenceContext(
        MaintenanceSchedule schedule) =>
        new(
            schedule.Id,
            schedule.AssetId,
            schedule.ScheduledDate,
            schedule.FrequencyDays,
            schedule.ResponsibleUserId,
            LimitText(schedule.Status, 80),
            LimitText(schedule.Observations, 240));

    private static TechnicalServiceRequestEvidenceContext ToTechnicalServiceRequestEvidenceContext(
        TechnicalServiceRequest request) =>
        new(
            request.Id,
            LimitText(request.Code, 80),
            request.AssetId,
            request.IncidentId,
            LimitText(request.AssetName, 120),
            LimitText(request.IssueDescription, 240),
            LimitText(request.Priority, 80),
            LimitText(request.Status, 80),
            request.RequestedAt,
            request.ClosedAt,
            LimitText(request.ClosureSummary, 240));

    private static string? LimitText(string? value, int maxLength)
    {
        if (value is null) return null;

        var normalized = value.Trim();
        if (normalized.Length <= maxLength) return normalized;
        return maxLength <= 3 ? normalized[..maxLength] : normalized[..(maxLength - 3)] + "...";
    }

    private static GenerateDashboardAiInterpretationError MapAiGenerationError(AiGenerationError error) =>
        error switch
        {
            AiGenerationError.ProviderDisabled => GenerateDashboardAiInterpretationError.AiProviderDisabled,
            AiGenerationError.ProviderNotConfigured => GenerateDashboardAiInterpretationError.AiProviderNotConfigured,
            AiGenerationError.ProviderUnavailable => GenerateDashboardAiInterpretationError.AiProviderUnavailable,
            AiGenerationError.ProviderTimeout => GenerateDashboardAiInterpretationError.AiProviderTimeout,
            AiGenerationError.InvalidStructuredOutput =>
                GenerateDashboardAiInterpretationError.InvalidStructuredOutput,
            _ => GenerateDashboardAiInterpretationError.UnexpectedError
        };

    private static Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError> Failure(
        GenerateDashboardAiInterpretationError error) =>
        new Result<DashboardAiInterpretation, GenerateDashboardAiInterpretationError>.Failure(error);

    private record DashboardInterpretationContext(
        string ContextVersion,
        int OrganizationId,
        string? OperatorQuestion,
        string ResponseLanguage,
        DashboardMetricContext Metrics,
        IReadOnlyCollection<DashboardSourceMetricOutput> SourceMetrics,
        IReadOnlyCollection<ReadingEvidenceContext> RecentReadings,
        IReadOnlyCollection<IncidentEvidenceContext> RecentIncidents,
        IReadOnlyCollection<AssetEvidenceContext> ReferencedAssets,
        IReadOnlyCollection<DeviceEvidenceContext> ReferencedDevices,
        IReadOnlyCollection<GatewayEvidenceContext> ReferencedGateways,
        IReadOnlyCollection<ReportEvidenceContext> RecentReports,
        MaintenanceEvidenceContext Maintenance,
        IReadOnlyCollection<string> EvidenceNotes);

    private record DashboardMetricContext(
        int MonitoredAssets,
        int ReadingsReviewed,
        int OutOfRangeReadings,
        double? ThermalCompliancePercentage,
        double? AverageTemperature,
        double? AverageHumidity,
        int Incidents,
        int OpenIncidents,
        int RecentReports,
        int MaintenanceSchedules,
        int TechnicalServiceRequests,
        int OpenTechnicalServiceRequests,
        int DevicesWithUnhealthyStatus,
        int GatewaysWithUnhealthyStatus);

    private record ReadingEvidenceContext(
        int Id,
        int AssetId,
        int IotDeviceId,
        int GatewayId,
        int LocationId,
        double? Temperature,
        double? Humidity,
        bool OutOfRange,
        DateTimeOffset RecordedAt);

    private record IncidentEvidenceContext(
        int Id,
        int? AssetId,
        string? Status,
        DateTimeOffset DetectedAt,
        bool Open);

    private record AssetEvidenceContext(int Id, int LocationId, string? Name);

    private record DeviceEvidenceContext(
        int Id,
        int GatewayId,
        int? AssetId,
        string? Name,
        string? Status,
        IReadOnlyCollection<string> MeasurementParameters);

    private record GatewayEvidenceContext(int Id, int LocationId, string? Status);

    private record ReportEvidenceContext(
        int Id,
        string? Uuid,
        string? Type,
        string? Title,
        DateTimeOffset GeneratedAt,
        int ReadingCount,
        int OutOfRangeReadingCount,
        int OpenIncidentCount,
        double? CompliancePercentage);

    private record MaintenanceEvidenceContext(
        IReadOnlyCollection<MaintenanceScheduleEvidenceContext> Schedules,
        IReadOnlyCollection<TechnicalServiceRequestEvidenceContext> TechnicalServiceRequests);

    private record MaintenanceScheduleEvidenceContext(
        int Id,
        int AssetId,
        DateTimeOffset ScheduledDate,
        int? FrequencyDays,
        int? ResponsibleUserId,
        string? Status,
        string? Observations);

    private record TechnicalServiceRequestEvidenceContext(
        int Id,
        string? Code,
        int AssetId,
        int? IncidentId,
        string? AssetName,
        string? IssueDescription,
        string? Priority,
        string? Status,
        DateTimeOffset RequestedAt,
        DateTimeOffset? ClosedAt,
        string? ClosureSummary);
}
