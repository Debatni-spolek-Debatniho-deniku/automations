namespace DSDD.Automations.Reports.Reports;

public interface IMemberFeesReport
{
    Task<Stream> GenerateXlsxAsync(CancellationToken ct);
}