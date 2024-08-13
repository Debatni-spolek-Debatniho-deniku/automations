namespace DSDD.Automations.Reports.Reports;

public interface IPayedTotalReport
{
    Task<Stream> GenerateXlsxAsync(ulong constantSymbol, CancellationToken ct);
}