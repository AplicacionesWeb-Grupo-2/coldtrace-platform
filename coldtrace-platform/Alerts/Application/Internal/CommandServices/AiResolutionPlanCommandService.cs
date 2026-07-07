using System.Text.Json;
using ColdTrace.Platform.AiAssistance.Application.Errors;
using ColdTrace.Platform.AiAssistance.Application.Prompts;
using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.Alerts.Application.Internal.CommandServices;

/// <summary>
///     Application service for AI resolution plan command operations.
/// </summary>
public class AiResolutionPlanCommandService(
    IAiResolutionPlanRepository aiResolutionPlanRepository,
    IIncidentRepository incidentRepository,
    INotificationRepository notificationRepository,
    IOrganizationRepository organizationRepository,
    IAssetRepository assetRepository,
    IIotDeviceRepository iotDeviceRepository,
    IAssetSettingsRepository assetSettingsRepository,
    ISensorReadingRepository sensorReadingRepository,
    IMaintenanceScheduleRepository maintenanceScheduleRepository,
    ITechnicalServiceRequestRepository technicalServiceRequestRepository,
    IAiStructuredOutputService aiStructuredOutputService,
    IOptions<AiOptions> aiOptions,
    IUnitOfWork unitOfWork,
    ILogger<AiResolutionPlanCommandService> logger)
    : IAiResolutionPlanCommandService
{
    private const int RecentReadingsLimit = 12;
    private const int MaintenanceContextLimit = 5;
    private const int TechnicalServiceRequestContextLimit = 5;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<AiResolutionPlan, ApproveAiResolutionPlanError>> Handle(
        ApproveAiResolutionPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for AI resolution plan approval: {OrganizationId}",
                command.OrganizationId);
            return ApprovalFailure(ApproveAiResolutionPlanError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for AI resolution plan approval: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return ApprovalFailure(ApproveAiResolutionPlanError.IncidentNotFound);
        }

        var plan = await aiResolutionPlanRepository.FindByIdAndIncidentIdAndOrganizationIdAsync(
            command.PlanId,
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (plan is null)
        {
            logger.LogWarning(
                "AI resolution plan not found for approval: {OrganizationId} {IncidentId} {PlanId}",
                command.OrganizationId,
                command.IncidentId,
                command.PlanId);
            return ApprovalFailure(ApproveAiResolutionPlanError.PlanNotFound);
        }

        if (!plan.IsPending())
        {
            logger.LogWarning(
                "AI resolution plan already decided: {OrganizationId} {IncidentId} {PlanId}",
                command.OrganizationId,
                command.IncidentId,
                command.PlanId);
            return ApprovalFailure(ApproveAiResolutionPlanError.PlanAlreadyDecided);
        }

        if (incident.IsResolved())
            return ApprovalFailure(ApproveAiResolutionPlanError.IncidentAlreadyResolved);

        if (!incident.IsOpen() && !incident.IsAcknowledged())
            return ApprovalFailure(ApproveAiResolutionPlanError.InvalidIncidentLifecycleTransition);

        try
        {
            incident.RegisterCorrectiveAction(
                new RegisterIncidentCorrectiveActionCommand(
                    command.OrganizationId,
                    command.IncidentId,
                    command.FinalCorrectiveAction,
                    command.ApprovedBy));
            incident.Resolve(
                new ResolveIncidentCommand(
                    command.OrganizationId,
                    command.IncidentId,
                    command.ApprovedBy,
                    command.FinalResolutionNotes));
            plan.Approve(command);

            var notification = Notification.IncidentResolved(incident);
            await notificationRepository.AddAsync(notification, cancellationToken);
            incident.RecordNotification(notification.Status);

            incidentRepository.Update(incident);
            aiResolutionPlanRepository.Update(plan);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<AiResolutionPlan, ApproveAiResolutionPlanError>.Success(plan);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed approving AI resolution plan {PlanId} for incident {IncidentId}",
                command.PlanId,
                command.IncidentId);
            return ApprovalFailure(ApproveAiResolutionPlanError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error approving AI resolution plan {PlanId} for incident {IncidentId}",
                command.PlanId,
                command.IncidentId);
            return ApprovalFailure(ApproveAiResolutionPlanError.UnexpectedError);
        }
    }

    public async Task<Result<AiResolutionPlan, GenerateAiResolutionPlanError>> Handle(
        GenerateAiResolutionPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for AI resolution plan generation: {OrganizationId}",
                command.OrganizationId);
            return Failure(GenerateAiResolutionPlanError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for AI resolution plan generation: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return Failure(GenerateAiResolutionPlanError.IncidentNotFound);
        }

        if (!incident.IsOpen() && !incident.IsAcknowledged())
            return Failure(GenerateAiResolutionPlanError.IncidentCannotReceivePlans);

        var contextResult = await BuildResolutionContextAsync(incident, cancellationToken);
        if (contextResult is Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError>.Failure failure)
            return Failure(failure.Error);

        var context = ((Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError>.Success)contextResult).Value;
        var contextJson = JsonSerializer.Serialize(context, SerializerOptions);
        var generationResult = await aiStructuredOutputService.GenerateIncidentResolutionPlanAsync(
            BuildPrompt(contextJson),
            cancellationToken);

        if (generationResult is Result<IncidentResolutionPlanOutput, AiGenerationError>.Failure aiFailure)
            return Failure(MapAiGenerationError(aiFailure.Error));

        var output = ((Result<IncidentResolutionPlanOutput, AiGenerationError>.Success)generationResult).Value;
        if (!IsValid(output))
            return Failure(GenerateAiResolutionPlanError.InvalidStructuredOutput);

        try
        {
            var plan = new AiResolutionPlan(ToCreateCommand(command, output));
            await aiResolutionPlanRepository.AddAsync(plan, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<AiResolutionPlan, GenerateAiResolutionPlanError>.Success(plan);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed persisting AI resolution plan for incident {IncidentId}",
                command.IncidentId);
            return Failure(GenerateAiResolutionPlanError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error persisting AI resolution plan for incident {IncidentId}",
                command.IncidentId);
            return Failure(GenerateAiResolutionPlanError.UnexpectedError);
        }
    }

    private async Task<Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError>> BuildResolutionContextAsync(
        Incident incident,
        CancellationToken cancellationToken)
    {
        SensorReading? triggerReading = null;
        if (incident.ReadingId is not null)
        {
            triggerReading = await sensorReadingRepository.FindByIdAndOrganizationIdAsync(
                incident.ReadingId.Value,
                incident.OrganizationId,
                cancellationToken);
            if (triggerReading is null)
                return ContextUnavailable(incident, "referenced reading was not found in organization");
        }

        var effectiveAssetId = incident.AssetId ?? triggerReading?.AssetId;
        var effectiveDeviceId = incident.DeviceId ?? triggerReading?.IotDeviceId;

        Asset? asset = null;
        if (effectiveAssetId is not null)
        {
            asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                effectiveAssetId.Value,
                incident.OrganizationId,
                cancellationToken);
            if (asset is null)
                return ContextUnavailable(incident, "referenced asset was not found in organization");
        }

        IotDevice? device = null;
        if (effectiveDeviceId is not null)
        {
            device = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
                effectiveDeviceId.Value,
                incident.OrganizationId,
                cancellationToken);
            if (device is null)
                return ContextUnavailable(incident, "referenced device was not found in organization");
            if (effectiveAssetId is not null && device.AssetId is not null && device.AssetId != effectiveAssetId)
                return ContextUnavailable(incident, "referenced device does not belong to incident asset");
        }

        if (triggerReading is not null)
        {
            if (effectiveAssetId is not null && triggerReading.AssetId != effectiveAssetId)
                return ContextUnavailable(incident, "referenced reading does not belong to incident asset");
            if (effectiveDeviceId is not null && triggerReading.IotDeviceId != effectiveDeviceId)
                return ContextUnavailable(incident, "referenced reading does not belong to incident device");
        }

        var settings = await FindEffectiveSettingsAsync(incident.OrganizationId, effectiveAssetId, cancellationToken);
        var recentReadings = await sensorReadingRepository.FindAllByOrganizationIdAsync(
            incident.OrganizationId,
            effectiveAssetId,
            effectiveDeviceId,
            cancellationToken: cancellationToken);
        var maintenanceSchedules = await maintenanceScheduleRepository.FindAllByOrganizationIdAsync(
            incident.OrganizationId,
            cancellationToken);
        var technicalServiceRequests = await technicalServiceRequestRepository.FindAllByOrganizationIdAsync(
            incident.OrganizationId,
            cancellationToken);

        return new Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError>.Success(
            new IncidentResolutionPlanContext(
                ToIncidentSnapshot(incident),
                asset is null ? null : ToAssetSnapshot(asset),
                device is null ? null : ToDeviceSnapshot(device),
                triggerReading is null ? null : ToReadingSnapshot(triggerReading),
                settings is null ? null : ToSafeRangeSnapshot(settings),
                recentReadings.Take(RecentReadingsLimit).Select(ToReadingSnapshot).ToList(),
                maintenanceSchedules
                    .Where(schedule => effectiveAssetId is null || schedule.AssetId == effectiveAssetId)
                    .OrderBy(schedule => schedule.ScheduledDate)
                    .Take(MaintenanceContextLimit)
                    .Select(ToMaintenanceSnapshot)
                    .ToList(),
                technicalServiceRequests
                    .Where(request => request.IncidentId == incident.Id ||
                                      (effectiveAssetId is not null && request.AssetId == effectiveAssetId))
                    .OrderByDescending(request => request.RequestedAt)
                    .Take(TechnicalServiceRequestContextLimit)
                    .Select(ToTechnicalServiceRequestSnapshot)
                    .ToList(),
                DateTimeOffset.UtcNow));
    }

    private async Task<AssetSettings?> FindEffectiveSettingsAsync(
        int organizationId,
        int? assetId,
        CancellationToken cancellationToken)
    {
        if (assetId is not null)
        {
            var assetSettings = await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                organizationId,
                assetId.Value,
                cancellationToken);
            if (assetSettings is not null) return assetSettings;
        }

        return await assetSettingsRepository.FindDefaultByOrganizationIdAsync(organizationId, cancellationToken);
    }

    private Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError> ContextUnavailable(
        Incident incident,
        string reason)
    {
        logger.LogWarning(
            "Incident context unavailable for AI resolution plan generation: {OrganizationId} {IncidentId}. Reason: {Reason}",
            incident.OrganizationId,
            incident.Id,
            reason);
        return new Result<IncidentResolutionPlanContext, GenerateAiResolutionPlanError>.Failure(
            GenerateAiResolutionPlanError.IncidentContextUnavailable);
    }

    private AiStructuredPrompt BuildPrompt(string contextJson) =>
        new(
            """
            You are ColdTrace's operations assistant for cold-chain incident resolution.
            Use only the provided incident context. Return a structured response with concise,
            auditable recommendations. Do not invent assets, devices, readings, maintenance records,
            people, or evidence that is absent from the context.
            """,
            """
            Generate an incident resolution plan draft for an operator. The plan must include a summary,
            probable cause, ordered recommended steps, corrective action draft, resolution notes draft,
            escalation recommendation, required evidence, and uncertainty notes.
            """,
            new Dictionary<string, string> { ["incidentResolutionContext"] = contextJson });

    private CreateAiResolutionPlanCommand ToCreateCommand(
        GenerateAiResolutionPlanCommand command,
        IncidentResolutionPlanOutput output)
    {
        var options = aiOptions.Value;
        return new CreateAiResolutionPlanCommand(
            command.OrganizationId,
            command.IncidentId,
            output.Summary,
            output.ProbableCause,
            output.RecommendedSteps
                .OrderBy(step => step.Sequence)
                .Select(step => new CreateAiResolutionPlanStepCommand(
                    step.Sequence,
                    step.Action,
                    step.Rationale,
                    step.ExpectedOutcome))
                .ToList(),
            output.CorrectiveActionDraft,
            output.ResolutionNotesDraft,
            output.EscalationRecommended,
            output.EscalationUrgency,
            output.EscalationReason,
            output.RequiredEvidence,
            output.UncertaintyNotes,
            options.Provider,
            options.Model ?? "not-configured",
            JsonSerializer.Serialize(new
            {
                contract = "incident-resolution-plan",
                contextVersion = "ts19-real-context-v1"
            }, SerializerOptions));
    }

    private static GenerateAiResolutionPlanError MapAiGenerationError(AiGenerationError error) =>
        error switch
        {
            AiGenerationError.ProviderDisabled => GenerateAiResolutionPlanError.AiProviderDisabled,
            AiGenerationError.ProviderNotConfigured => GenerateAiResolutionPlanError.AiProviderNotConfigured,
            AiGenerationError.ProviderUnavailable => GenerateAiResolutionPlanError.AiProviderUnavailable,
            AiGenerationError.ProviderTimeout => GenerateAiResolutionPlanError.AiProviderTimeout,
            AiGenerationError.InvalidStructuredOutput => GenerateAiResolutionPlanError.InvalidStructuredOutput,
            _ => GenerateAiResolutionPlanError.UnexpectedError
        };

    private static bool IsValid(IncidentResolutionPlanOutput output) =>
        !IsBlank(output.Summary) &&
        !IsBlank(output.ProbableCause) &&
        output.RecommendedSteps is { Count: > 0 } &&
        output.RecommendedSteps.All(step =>
            step.Sequence > 0 &&
            !IsBlank(step.Action) &&
            !IsBlank(step.Rationale) &&
            !IsBlank(step.ExpectedOutcome)) &&
        !IsBlank(output.CorrectiveActionDraft) &&
        !IsBlank(output.ResolutionNotesDraft) &&
        !IsBlank(output.EscalationUrgency) &&
        !IsBlank(output.EscalationReason) &&
        output.RequiredEvidence is not null &&
        output.UncertaintyNotes is not null;

    private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);

    private static Result<AiResolutionPlan, GenerateAiResolutionPlanError> Failure(
        GenerateAiResolutionPlanError error) =>
        new Result<AiResolutionPlan, GenerateAiResolutionPlanError>.Failure(error);

    private static Result<AiResolutionPlan, ApproveAiResolutionPlanError> ApprovalFailure(
        ApproveAiResolutionPlanError error) =>
        new Result<AiResolutionPlan, ApproveAiResolutionPlanError>.Failure(error);

    private static IncidentSnapshot ToIncidentSnapshot(Incident incident) =>
        new(
            incident.Id,
            incident.OrganizationId,
            incident.AssetId,
            incident.DeviceId,
            incident.ReadingId,
            incident.AssetName,
            incident.DeviceName,
            incident.Type,
            incident.Severity,
            incident.Status,
            incident.Value,
            incident.DetectedAt,
            incident.AcknowledgedAt,
            incident.EscalatedAt,
            incident.EscalationReason,
            incident.CorrectiveAction,
            incident.NotificationCount);

    private static AssetSnapshot ToAssetSnapshot(Asset asset) =>
        new(asset.Id, asset.LocationId, asset.Uuid, asset.Type, asset.Name, asset.Capacity, asset.Status);

    private static IotDeviceSnapshot ToDeviceSnapshot(IotDevice device) =>
        new(
            device.Id,
            device.AssetId,
            device.GatewayId,
            device.Uuid,
            device.Name,
            device.DeviceType,
            device.Model,
            device.MeasurementType,
            device.MeasurementParameters,
            device.ReadingFrequencySeconds,
            device.Status,
            device.CalibrationStatus,
            device.LastCalibrationDate,
            device.NextCalibrationDate);

    private static SensorReadingSnapshot ToReadingSnapshot(SensorReading reading) =>
        new(
            reading.Id,
            reading.AssetId,
            reading.IotDeviceId,
            reading.GatewayId,
            reading.LocationId,
            reading.Temperature,
            reading.Humidity,
            reading.OutOfRange,
            reading.RecordedAt,
            reading.MotionDetected,
            reading.ImageCaptured,
            reading.BatteryLevel,
            reading.SignalStrength);

    private static SafeRangeSnapshot ToSafeRangeSnapshot(AssetSettings settings) =>
        new(
            settings.Id,
            settings.AssetId,
            settings.MinimumTemperature,
            settings.MaximumTemperature,
            settings.MinimumHumidity,
            settings.MaximumHumidity,
            settings.TemperatureUnit,
            settings.HumidityUnit,
            settings.ReadingFrequencySeconds,
            settings.AlertThresholdMinutes);

    private static MaintenanceScheduleSnapshot ToMaintenanceSnapshot(MaintenanceSchedule schedule) =>
        new(
            schedule.Id,
            schedule.AssetId,
            schedule.ScheduledDate,
            schedule.FrequencyDays,
            schedule.ResponsibleUserId,
            schedule.Observations,
            schedule.Status,
            schedule.IsActive());

    private static TechnicalServiceRequestSnapshot ToTechnicalServiceRequestSnapshot(
        TechnicalServiceRequest request) =>
        new(
            request.Id,
            request.Code,
            request.AssetId,
            request.AssetLocationId,
            request.AssetName,
            request.IncidentId,
            request.IssueDescription,
            request.Priority,
            request.Status,
            request.RequestedBy,
            request.RequestedAt,
            request.ClosedAt,
            request.ClosureSummary,
            request.Evidence,
            request.IsActive());

    private record IncidentResolutionPlanContext(
        IncidentSnapshot Incident,
        AssetSnapshot? Asset,
        IotDeviceSnapshot? Device,
        SensorReadingSnapshot? TriggerReading,
        SafeRangeSnapshot? SafeRange,
        IReadOnlyCollection<SensorReadingSnapshot> RecentReadings,
        IReadOnlyCollection<MaintenanceScheduleSnapshot> MaintenanceSchedules,
        IReadOnlyCollection<TechnicalServiceRequestSnapshot> TechnicalServiceRequests,
        DateTimeOffset RequestedAt);

    private record IncidentSnapshot(
        int Id,
        int OrganizationId,
        int? AssetId,
        int? DeviceId,
        int? ReadingId,
        string? AssetName,
        string? DeviceName,
        string Type,
        string Severity,
        string Status,
        string? Value,
        DateTimeOffset DetectedAt,
        DateTimeOffset? AcknowledgedAt,
        DateTimeOffset? EscalatedAt,
        string? EscalationReason,
        string? CorrectiveAction,
        int NotificationCount);

    private record AssetSnapshot(
        int Id,
        int LocationId,
        string Uuid,
        string Type,
        string Name,
        double Capacity,
        string Status);

    private record IotDeviceSnapshot(
        int Id,
        int? AssetId,
        int GatewayId,
        string Uuid,
        string Name,
        string DeviceType,
        string Model,
        string MeasurementType,
        IReadOnlyList<string> MeasurementParameters,
        int ReadingFrequencySeconds,
        string Status,
        string CalibrationStatus,
        DateOnly LastCalibrationDate,
        DateOnly NextCalibrationDate);

    private record SensorReadingSnapshot(
        int Id,
        int AssetId,
        int IotDeviceId,
        int GatewayId,
        int LocationId,
        double? Temperature,
        double? Humidity,
        bool OutOfRange,
        DateTimeOffset RecordedAt,
        bool? MotionDetected,
        bool? ImageCaptured,
        int? BatteryLevel,
        int? SignalStrength);

    private record SafeRangeSnapshot(
        int Id,
        int? AssetId,
        double MinimumTemperature,
        double MaximumTemperature,
        double MinimumHumidity,
        double MaximumHumidity,
        string TemperatureUnit,
        string HumidityUnit,
        int ReadingFrequencySeconds,
        int AlertThresholdMinutes);

    private record MaintenanceScheduleSnapshot(
        int Id,
        int AssetId,
        DateTimeOffset ScheduledDate,
        int? FrequencyDays,
        int? ResponsibleUserId,
        string? Observations,
        string Status,
        bool Active);

    private record TechnicalServiceRequestSnapshot(
        int Id,
        string Code,
        int AssetId,
        int AssetLocationId,
        string? AssetName,
        int? IncidentId,
        string IssueDescription,
        string Priority,
        string Status,
        string? RequestedBy,
        DateTimeOffset RequestedAt,
        DateTimeOffset? ClosedAt,
        string? ClosureSummary,
        string? Evidence,
        bool Active);
}
