using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Reports.Domain.Services;

/// <summary>
///     Application service contract for report commands.
/// </summary>
public interface IReportCommandService
{
    Task<Result<Report, GenerateReportError>> Handle(
        GenerateReportCommand command,
        CancellationToken cancellationToken = default);
}
