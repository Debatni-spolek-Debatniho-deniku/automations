namespace DSDD.Automations.Payments.Payments;

public interface IPaymentsService
{
    Task<object> GetPaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct);

    Task UpsertManualPaymentAsync(ulong variableSymbol, string? paymentReference, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description, CancellationToken ct);

    Task OverrideBankPaymentAsync(ulong variableSymbol, string paymentReference, ulong? constantSymbol, DateTime? dateTime, string? description, CancellationToken ct);

    Task RemovePaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct);

    Task RestorePaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct);
}