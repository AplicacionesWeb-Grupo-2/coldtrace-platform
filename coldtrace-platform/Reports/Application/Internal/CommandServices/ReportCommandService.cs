using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
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
            var incidentsInPeriod = incidents
                .Where(incident => incident.DetectedAt >= command.PeriodStart && incident.DetectedAt <= command.PeriodEnd)
                .ToList();

            var report = new Report(
                command,
                assets.Count(),
                readingCount: 0,
                outOfRangeReadingCount: 0,
                incidentsInPeriod.Count,
                incidentsInPeriod.Count(IsOpenIncident),
                averageTemperature: null,
                averageHumidity: null,
                compliancePercentage: null);

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
