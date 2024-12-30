namespace DSDD.Automations.Reports.Reports.PayedTotal;

public interface IPayedTotalReport
{
    Task<ReportFile> GenerateXlsxAsync(ulong constantSymbol, CancellationToken ct);
}