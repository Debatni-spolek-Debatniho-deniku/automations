namespace DSDD.Automations.Reports.Reports.MemberFees;

public interface IMemberFeesReport
{
    Task<ReportFile> GenerateXlsxAsync(CancellationToken ct);
}