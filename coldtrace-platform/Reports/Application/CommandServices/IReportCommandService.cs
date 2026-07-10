using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Reports.Application.CommandServices;

/// <summary>
///     Application service contract for report commands.
/// </summary>
public interface IReportCommandService
{
    Task<Result<Report, GenerateReportError>> Handle(
        GenerateReportCommand command,
        CancellationToken cancellationToken = default);
}
