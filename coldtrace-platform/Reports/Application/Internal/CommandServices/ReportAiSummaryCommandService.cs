using System.Text.Json;
using System.Text.RegularExpressions;
using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Application.Results;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Reports.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.Reports.Application.Internal.CommandServices;

/// <summary>
///     Application service that generates advisory report summaries from backend-owned evidence.
/// </summary>
public class ReportAiSummaryCommandService(
    IReportRepository reportRepository,
    IOrganizationRepository organizationRepository,
    IAssetRepository assetRepository,
    IIncidentRepository incidentRepository,
    ISensorReadingRepository sensorReadingRepository,
    ITechnicalServiceRequestRepository technicalServiceRequestRepository,
    IAiStructuredOutputService aiStructuredOutputService,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IOptions<AiOptions> aiOptions,
    ILogger<ReportAiSummaryCommandService> logger)
    : IReportAiSummaryCommandService
{
    private const int MaxReadingEvidence = 12;
    private const int MaxIncidentEvidence = 10;
    private const int MaxAssetEvidence = 10;
    private const int MaxCorrectiveActionEvidence = 8;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<ReportAiSummary, GenerateReportAiSummaryError>> Handle(
        GenerateReportAiSummaryCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for AI report summary: {OrganizationId}",
                command.OrganizationId);
            return Failure(GenerateReportAiSummaryError.OrganizationNotFound);
        }

        var report = await reportRepository.FindByIdAndOrganizationIdAsync(
            command.ReportId,
            command.OrganizationId,
            cancellationToken);
        if (report is null)
        {
            logger.LogWarning(
                "Report not found for AI summary: {OrganizationId} {ReportId}",
                command.OrganizationId,
                command.ReportId);
            return Failure(GenerateReportAiSummaryError.ReportNotFound);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementAiReportSummary,
            "ReportAiSummaryPlanLimitExceeded",
            cancellationToken);

        try
        {
            var context = await BuildReportSummaryContextAsync(command.OrganizationId, report, cancellationToken);
            var serializedContext = JsonSerializer.Serialize(context, SerializerOptions);
            var generationResult = await aiStructuredOutputService.GenerateComplianceSummaryAsync(
                BuildPrompt(serializedContext),
                cancellationToken);

            if (generationResult is Result<ComplianceSummaryOutput, AiGenerationError>.Failure aiFailure)
                return Failure(MapAiGenerationError(aiFailure.Error));

            var output = ((Result<ComplianceSummaryOutput, AiGenerationError>.Success)generationResult).Value;
            var summary = SanitizeComplianceSummary(output, context);
            var options = aiOptions.Value;

            logger.LogInformation(
                "AI report summary generated: {OrganizationId} {ReportId} {ModelProvider} {ModelName}",
                command.OrganizationId,
                command.ReportId,
                options.Provider,
                options.Model);

            return new Result<ReportAiSummary, GenerateReportAiSummaryError>.Success(
                new ReportAiSummary(
                    report,
                    summary,
                    options.Provider,
                    options.Model ?? "not-configured",
                    DateTimeOffset.UtcNow));
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Report context serialization failed for AI summary");
            return Failure(GenerateReportAiSummaryError.ReportContextUnavailable);
        }
        catch (NotSupportedException ex)
        {
            logger.LogWarning(ex, "Report context could not be serialized for AI summary");
            return Failure(GenerateReportAiSummaryError.ReportContextUnavailable);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error generating AI report summary for report {ReportId}",
                command.ReportId);
            return Failure(GenerateReportAiSummaryError.UnexpectedError);
        }
    }

    private async Task<ReportSummaryContext> BuildReportSummaryContextAsync(
        int organizationId,
        Report report,
        CancellationToken cancellationToken)
    {
        var readings = (await sensorReadingRepository.FindAllByOrganizationIdAsync(
                organizationId,
                from: report.PeriodStart,
                to: report.PeriodEnd,
                cancellationToken: cancellationToken))
            .ToList();
        var incidents = (await incidentRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .Where(incident => IsInReportPeriod(incident.DetectedAt, report))
            .ToList();
        var assetIds = readings
            .Select(reading => reading.AssetId)
            .Concat(incidents.Select(incident => incident.AssetId).OfType<int>())
            .Distinct()
            .ToHashSet();
        var affectedAssets = (await assetRepository.FindAllByOrganizationIdAsync(organizationId, cancellationToken))
            .Where(asset => assetIds.Contains(asset.Id))
            .OrderBy(asset => asset.Id)
            .ToList();
        var incidentIds = incidents.Select(incident => incident.Id).ToHashSet();
        var correctiveActions = (await technicalServiceRequestRepository.FindAllByOrganizationIdAsync(
                organizationId,
                cancellationToken))
            .Where(request =>
                (request.IncidentId is not null && incidentIds.Contains(request.IncidentId.Value)) ||
                assetIds.Contains(request.AssetId))
            .OrderByDescending(request => request.RequestedAt)
            .ToList();

        return new ReportSummaryContext(
            "report-ai-summary.v1",
            ToReportContext(report),
            ToReportMetricsContext(report),
            readings
                .Where(reading => reading.OutOfRange)
                .OrderByDescending(reading => reading.RecordedAt)
                .Take(MaxReadingEvidence)
                .Select(ToReadingEvidenceContext)
                .ToList(),
            incidents
                .OrderByDescending(incident => incident.DetectedAt)
                .Take(MaxIncidentEvidence)
                .Select(ToIncidentEvidenceContext)
                .ToList(),
            affectedAssets
                .Take(MaxAssetEvidence)
                .Select(ToAssetEvidenceContext)
                .ToList(),
            correctiveActions
                .Take(MaxCorrectiveActionEvidence)
                .Select(ToCorrectiveActionEvidenceContext)
                .ToList(),
            BuildEvidenceNotes(report, readings, incidents, correctiveActions));
    }

    private static bool IsInReportPeriod(DateTimeOffset value, Report report) =>
        value >= report.PeriodStart && value <= report.PeriodEnd;

    private static IReadOnlyCollection<string> BuildEvidenceNotes(
        Report report,
        IReadOnlyCollection<SensorReading> readings,
        IReadOnlyCollection<Incident> incidents,
        IReadOnlyCollection<TechnicalServiceRequest> correctiveActions)
    {
        var notes = new List<string>();
        if (report.ReadingCount == 0 || readings.Count == 0)
            notes.Add("No sensor readings were available in the report period.");
        if (report.OutOfRangeReadingCount > 0 && readings.All(reading => !reading.OutOfRange))
            notes.Add("The report stores out-of-range metrics, but detailed out-of-range readings are unavailable.");
        if (report.IncidentCount > 0 && incidents.Count == 0)
            notes.Add("The report stores incident metrics, but detailed incident records are unavailable.");
        if (incidents.Count > 0 && correctiveActions.Count == 0)
            notes.Add("No corrective action evidence was found for incidents or affected assets in this report.");
        if (report.CompliancePercentage is null)
            notes.Add("Compliance percentage is unavailable because the report has no eligible readings.");
        return notes;
    }

    private static AiStructuredPrompt BuildPrompt(string contextJson) =>
        new(
            """
            You are ColdTrace's compliance assistant for cold-chain report review.
            Use only the provided report context. Return a structured advisory response with concise,
            auditable findings. Do not invent assets, readings, incidents, corrective actions, people,
            or evidence that is absent from the context.
            """,
            """
            Generate an intelligent compliance summary for a quality lead. The summary must include an
            executive summary, structured findings, evidence gaps, recommended actions, and uncertainty notes.
            Treat persisted report metrics as the factual source of truth.
            """,
            new Dictionary<string, string> { ["reportSummaryContext"] = contextJson });

    private ComplianceSummaryOutput SanitizeComplianceSummary(
        ComplianceSummaryOutput summary,
        ReportSummaryContext context) =>
        new(
            SanitizeText(summary.ExecutiveSummary, 600, FallbackExecutiveSummary())!,
            SanitizeFindings(summary.Findings, context),
            SanitizeTextList(summary.EvidenceGaps, FallbackEvidenceGaps(), 2, 2, 240),
            SanitizeTextList(summary.RecommendedActions, FallbackRecommendedActions(), 2, 2, 240),
            SanitizeTextList(summary.UncertaintyNotes, FallbackUncertaintyNotes(), 1, 1, 240));

    private IReadOnlyCollection<ComplianceFindingOutput> SanitizeFindings(
        IReadOnlyCollection<ComplianceFindingOutput>? findings,
        ReportSummaryContext context)
    {
        var sanitized = new List<ComplianceFindingOutput>();
        if (findings is not null)
        {
            foreach (var finding in findings)
            {
                var sanitizedFinding = SanitizeFinding(finding);
                if (sanitizedFinding is not null)
                    sanitized.Add(sanitizedFinding);
                if (sanitized.Count == 3)
                    break;
            }
        }

        foreach (var fallback in FallbackFindings(context))
        {
            if (sanitized.Count == 3)
                break;
            sanitized.Add(fallback);
        }

        return sanitized;
    }

    private static ComplianceFindingOutput? SanitizeFinding(ComplianceFindingOutput finding)
    {
        var area = SanitizeText(finding.Area, 100, null);
        var status = SanitizeText(finding.Status, 80, null);
        var evidence = SanitizeText(finding.Evidence, 360, null);
        var recommendation = SanitizeText(finding.Recommendation, 320, null);
        return area is null || status is null || evidence is null || recommendation is null
            ? null
            : new ComplianceFindingOutput(area, status, evidence, recommendation);
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
                if (sanitizedValue is not null)
                    sanitized.Add(sanitizedValue);
                if (sanitized.Count == maximumSize)
                    break;
            }
        }

        foreach (var fallback in fallbacks)
        {
            if (sanitized.Count >= minimumSize)
                break;
            var sanitizedFallback = SanitizeText(fallback, maximumTextLength, null);
            if (sanitizedFallback is not null)
                sanitized.Add(sanitizedFallback);
        }

        return sanitized.Take(maximumSize).ToList();
    }

    private static string? SanitizeText(string? value, int maxLength, string? fallback)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        normalized = normalized
            .Replace("&#x0A;", " ", StringComparison.Ordinal)
            .Replace("\\n", " ", StringComparison.Ordinal);
        normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
        return LimitText(normalized, maxLength);
    }

    private static string FallbackExecutiveSummary() =>
        "The report requires operational review using persisted metrics and available evidence.";

    private static IReadOnlyCollection<ComplianceFindingOutput> FallbackFindings(ReportSummaryContext context) =>
        [
            new ComplianceFindingOutput(
                "Thermal compliance",
                FallbackStatus(context.Metrics.CompliancePercentage),
                "Compliance is based on persisted report metrics.",
                "Review out-of-range readings and confirm corrective evidence."),
            new ComplianceFindingOutput(
                "Incident pressure",
                context.Metrics.OpenIncidentCount > 0 ? "attention required" : "stable",
                $"The report records {context.Metrics.OpenIncidentCount} open incident(s).",
                "Prioritize open incident review before final compliance closure."),
            new ComplianceFindingOutput(
                "Evidence coverage",
                context.CorrectiveActions.Count == 0 ? "limited evidence" : "evidence available",
                "Corrective action evidence was assembled from backend records.",
                "Attach final corrective notes where evidence is incomplete.")
        ];

    private static string FallbackStatus(double? compliancePercentage) =>
        compliancePercentage is null
            ? "limited evidence"
            : compliancePercentage >= 95.0
                ? "stable"
                : "attention required";

    private static IReadOnlyCollection<string> FallbackEvidenceGaps() =>
        [
            "AI summary is limited to persisted report metrics and related backend evidence.",
            "Manual quality validation is still required before using the summary as final compliance evidence."
        ];

    private static IReadOnlyCollection<string> FallbackRecommendedActions() =>
        [
            "Review out-of-range readings and open incidents referenced by the report.",
            "Attach corrective evidence before sharing the report with quality stakeholders."
        ];

    private static IReadOnlyCollection<string> FallbackUncertaintyNotes() =>
        ["Generated summary is advisory and depends on the completeness of persisted ColdTrace records."];

    private static GenerateReportAiSummaryError MapAiGenerationError(AiGenerationError error) =>
        error switch
        {
            AiGenerationError.ProviderDisabled => GenerateReportAiSummaryError.AiProviderDisabled,
            AiGenerationError.ProviderNotConfigured => GenerateReportAiSummaryError.AiProviderNotConfigured,
            AiGenerationError.ProviderUnavailable => GenerateReportAiSummaryError.AiProviderUnavailable,
            AiGenerationError.ProviderTimeout => GenerateReportAiSummaryError.AiProviderTimeout,
            AiGenerationError.InvalidStructuredOutput => GenerateReportAiSummaryError.InvalidStructuredOutput,
            _ => GenerateReportAiSummaryError.UnexpectedError
        };

    private static Result<ReportAiSummary, GenerateReportAiSummaryError> Failure(GenerateReportAiSummaryError error) =>
        new Result<ReportAiSummary, GenerateReportAiSummaryError>.Failure(error);

    private static ReportContext ToReportContext(Report report) =>
        new(
            report.Id,
            report.OrganizationId,
            LimitText(report.Uuid, 80),
            LimitText(report.Type, 80),
            LimitText(report.Title, 160),
            report.PeriodStart,
            report.PeriodEnd,
            report.GeneratedAt);

    private static ReportMetricsContext ToReportMetricsContext(Report report) =>
        new(
            report.AssetCount,
            report.ReadingCount,
            report.OutOfRangeReadingCount,
            report.IncidentCount,
            report.OpenIncidentCount,
            report.AverageTemperature,
            report.AverageHumidity,
            report.CompliancePercentage);

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
            incident.IsOpen());

    private static AssetEvidenceContext ToAssetEvidenceContext(Asset asset) =>
        new(asset.Id, asset.LocationId, LimitText(asset.Name, 120));

    private static CorrectiveActionEvidenceContext ToCorrectiveActionEvidenceContext(
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
            LimitText(request.ClosureSummary, 240),
            LimitText(request.Evidence, 240),
            LimitText(request.ClosedBy, 120));

    private static string? LimitText(string? value, int maxLength) =>
        value is null || value.Length <= maxLength ? value : value[..maxLength];

    private record ReportSummaryContext(
        string ContextVersion,
        ReportContext Report,
        ReportMetricsContext Metrics,
        IReadOnlyCollection<ReadingEvidenceContext> OutOfRangeReadings,
        IReadOnlyCollection<IncidentEvidenceContext> Incidents,
        IReadOnlyCollection<AssetEvidenceContext> AffectedAssets,
        IReadOnlyCollection<CorrectiveActionEvidenceContext> CorrectiveActions,
        IReadOnlyCollection<string> EvidenceNotes);

    private record ReportContext(
        int Id,
        int OrganizationId,
        string? Uuid,
        string? Type,
        string? Title,
        DateTimeOffset PeriodStart,
        DateTimeOffset PeriodEnd,
        DateTimeOffset GeneratedAt);

    private record ReportMetricsContext(
        int AssetCount,
        int ReadingCount,
        int OutOfRangeReadingCount,
        int IncidentCount,
        int OpenIncidentCount,
        double? AverageTemperature,
        double? AverageHumidity,
        double? CompliancePercentage);

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

    private record CorrectiveActionEvidenceContext(
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
        string? ClosureSummary,
        string? Evidence,
        string? ClosedBy);
}
