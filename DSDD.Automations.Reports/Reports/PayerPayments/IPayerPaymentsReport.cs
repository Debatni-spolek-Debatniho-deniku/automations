namespace DSDD.Automations.Reports.Reports.PayerPayments;

public interface IPayerPaymentsReport
{
    Task<ReportFile> GenerateXlsxAsync(ulong variableSymbol, ulong? constantSymbol, CancellationToken ct);
}