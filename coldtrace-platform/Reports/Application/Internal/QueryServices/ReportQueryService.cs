using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Queries;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Reports.Application.CommandServices;
using ColdTrace.Platform.Reports.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Reports.Application.Internal.QueryServices;

/// <summary>
///     Application service for report query operations.
/// </summary>
public class ReportQueryService(
    IReportRepository reportRepository,
    IIamContextFacade iamContextFacade,
    ILogger<ReportQueryService> logger)
    : IReportQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Report>, GetReportsByOrganizationError>> Handle(
        GetReportsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for report query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Report>, GetReportsByOrganizationError>.Failure(
                GetReportsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var reports = await reportRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Report>, GetReportsByOrganizationError>.Success(reports);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting reports for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Report>, GetReportsByOrganizationError>.Failure(
                GetReportsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Report, GetReportByIdAndOrganizationError>> Handle(
        GetReportByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for report detail query: {OrganizationId}",
                query.OrganizationId);
            return new Result<Report, GetReportByIdAndOrganizationError>.Failure(
                GetReportByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var report = await reportRepository.FindByIdAndOrganizationIdAsync(
                query.ReportId,
                query.OrganizationId,
                cancellationToken);
            if (report is null)
                return new Result<Report, GetReportByIdAndOrganizationError>.Failure(
                    GetReportByIdAndOrganizationError.ReportNotFound);

            return new Result<Report, GetReportByIdAndOrganizationError>.Success(report);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error getting report {ReportId} for organization {OrganizationId}",
                query.ReportId,
                query.OrganizationId);
            return new Result<Report, GetReportByIdAndOrganizationError>.Failure(
                GetReportByIdAndOrganizationError.UnexpectedError);
        }
    }
}
