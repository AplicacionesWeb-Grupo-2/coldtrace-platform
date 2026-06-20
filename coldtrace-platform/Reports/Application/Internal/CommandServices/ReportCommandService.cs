using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Reports.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Reports.Application.Internal.CommandServices;

/// <summary>
///     Application service for report generation.
/// </summary>
public class ReportCommandService(
    IReportRepository reportRepository,
    IOrganizationRepository organizationRepository,
    IAssetRepository assetRepository,
    IIncidentRepository incidentRepository,
    ISensorReadingRepository sensorReadingRepository,
    IUnitOfWork unitOfWork,
    ILogger<ReportCommandService> logger)
    : IReportCommandService
{
    /// <inheritdoc />
    public async Task<Result<Report, GenerateReportError>> Handle(
        GenerateReportCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for report generation: {OrganizationId}",
                command.OrganizationId);
            return new Result<Report, GenerateReportError>.Failure(GenerateReportError.OrganizationNotFound);
        }

        try
        {
            var assets = await assetRepository.FindAllByOrganizationIdAsync(command.OrganizationId, cancellationToken);
            var incidents = await incidentRepository.FindAllByOrganizationIdAsync(
                command.OrganizationId,
                cancellationToken);
            var readings = (await sensorReadingRepository.FindAllByOrganizationIdAsync(
                    command.OrganizationId,
                    from: command.PeriodStart,
                    to: command.PeriodEnd,
                    cancellationToken: cancellationToken))
                .ToList();
            var incidentsInPeriod = incidents
                .Where(incident => incident.DetectedAt >= command.PeriodStart && incident.DetectedAt <= command.PeriodEnd)
                .ToList();
            var temperatureReadings = readings
                .Where(reading => reading.Temperature.HasValue)
                .Select(reading => reading.Temperature!.Value)
                .ToList();
            var humidityReadings = readings
                .Where(reading => reading.Humidity.HasValue)
                .Select(reading => reading.Humidity!.Value)
                .ToList();
            var outOfRangeReadingCount = readings.Count(reading => reading.OutOfRange);
            double? compliancePercentage = readings.Count == 0
                ? null
                : Math.Round(
                    (readings.Count - outOfRangeReadingCount) * 100.0 / readings.Count,
                    1,
                    MidpointRounding.AwayFromZero);
            double? averageTemperature = temperatureReadings.Count == 0
                ? null
                : Math.Round(temperatureReadings.Average(), 1, MidpointRounding.AwayFromZero);
            double? averageHumidity = humidityReadings.Count == 0
                ? null
                : Math.Round(humidityReadings.Average(), 1, MidpointRounding.AwayFromZero);
            var assetCount = readings.Count == 0
                ? assets.Count()
                : readings.Select(reading => reading.AssetId).Distinct().Count();

            var report = new Report(
                command,
                assetCount,
                readings.Count,
                outOfRangeReadingCount,
                incidentsInPeriod.Count,
                incidentsInPeriod.Count(IsOpenIncident),
                averageTemperature,
                averageHumidity,
                compliancePercentage);

            await reportRepository.AddAsync(report, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<Report, GenerateReportError>.Success(report);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed generating report for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<Report, GenerateReportError>.Failure(GenerateReportError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error generating report for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<Report, GenerateReportError>.Failure(GenerateReportError.UnexpectedError);
        }
    }

    private static bool IsOpenIncident(Incident incident) => !incident.IsResolved();
}
