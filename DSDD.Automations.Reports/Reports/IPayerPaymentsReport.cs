namespace DSDD.Automations.Reports.Reports;

public interface IPayerPaymentsReport
{
    Task<Stream> GenerateXlsxAsync(ulong variableSymbol, ulong? constantSymbol, CancellationToken ct);
}