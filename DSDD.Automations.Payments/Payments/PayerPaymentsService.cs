using DSDD.Automations.Payments.Persistence.Abstractions;
using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

namespace DSDD.Automations.Payments.Payments;

public class PayerPaymentsService: IPaymentsService
{
    public PayerPaymentsService(IPayersDao payers)
    {
        _payers = payers;
    }

    public async Task<object> GetPaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol, ct);
        return FilterOutPayment(payer, paymentReference, true)!;
    }

    public async Task UpsertManualPaymentAsync(ulong variableSymbol, string? paymentReference,
        ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description, CancellationToken ct)
    {
        Payer? payer = await _payers.GetAsync(variableSymbol, ct);
        payer ??= new(variableSymbol);

        if (paymentReference is not null)
        {
            PayerManualPayment[] concurrent = payer.ManualPayments.Where(p => p.Reference == paymentReference).ToArray();
            foreach (PayerManualPayment c in concurrent)
                payer.ManualPayments.Remove(c);
        }

        payer.ManualPayments.Add(new(
            paymentReference ?? Guid.NewGuid().ToString(), 
            constantSymbol, 
            amountCzk, 
            dateTime, 
            description));

        await _payers.UpsertAync(payer, ct);
    }

    public async Task OverrideBankPaymentAsync(ulong variableSymbol, string paymentReference, 
        ulong? constantSymbol, DateTime? dateTime, string? description, CancellationToken ct)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol, ct);

        PayerBankPayment payment = payer.BankPayments.Single(p => p.Reference == paymentReference);
        payment.Overrides.ConstantSymbol = constantSymbol;
        payment.Overrides.DateTime = dateTime;
        payment.Overrides.Description = description;

        await _payers.UpsertAync(payer, ct);
    }

    public async Task RemovePaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol, ct);
        
        switch (FilterOutPayment(payer, paymentReference, true)!)
        {
            case PayerBankPayment bankPayment:
                bankPayment.Overrides.Removed = true;
                break;
            case PayerManualPayment manualPayment:
                payer.ManualPayments.Remove(manualPayment);
                break;
            default:
                throw new IndexOutOfRangeException();
        }

        await _payers.UpsertAync(payer, ct);
    }

    public async Task RestorePaymentAsync(ulong variableSymbol, string paymentReference, CancellationToken ct)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol, ct);

        object? payment = FilterOutPayment(payer, paymentReference, false);
        if (payment is not PayerBankPayment bankPayment)
            throw new InvalidOperationException("Je možné obnovit pouze bankovní plataby!");

        bankPayment.Overrides.Removed = false;

        await _payers.UpsertAync(payer, ct);
    }

    private readonly IPayersDao _payers;

    private object? FilterOutPayment(Payer payer, string paymentReference, bool throwIfNotFound)
    {
        PayerBankPayment? maybeBankPayment = payer.BankPayments.SingleOrDefault(p => p.Reference == paymentReference);
        if (maybeBankPayment is not null)
            return maybeBankPayment;

        PayerManualPayment? maybeManualPayment = payer.ManualPayments.SingleOrDefault(p => p.Reference == paymentReference);
        if (maybeManualPayment is not null)
            return maybeManualPayment;

        if (throwIfNotFound)
            throw new NullReferenceException(
                $"Pro poplatníka {payer.VariableSymbol} neexistuje platba {paymentReference}!");

        return null;
    }
}