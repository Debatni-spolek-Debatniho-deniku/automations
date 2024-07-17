namespace DSDD.Automations.Payments.Payments;

public interface IPaymentsService
{
    Task<object> GetPaymentAsync(ulong variableSymbol, string paymentReference);

    Task UpsertManualPaymentAsync(ulong variableSymbol, string? paymentReference, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description);

    Task OverrideBankPaymentAsync(ulong variableSymbol, string paymentReference, ulong? constantSymbol, DateTime? dateTime, string? description);

    Task RemovePaymentAsync(ulong variableSymbol, string paymentReference);

    Task RestorePaymentAsync(ulong variableSymbol, string paymentReference);
}