using System.Diagnostics.CodeAnalysis;
using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments.Payments;

public class PayerPaymentsService: IPaymentsService
{
    public PayerPaymentsService(IPayersDao payers)
    {
        _payers = payers;
    }

    public async Task<object> GetPaymentAsync(ulong variableSymbol, string paymentReference)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol);
        return FilterOutPayment(payer, paymentReference, true)!;
    }

    public async Task UpsertManualPaymentAsync(ulong variableSymbol, string? paymentReference,
        ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description)
    {
        Payer? payer = await _payers.GetAsync(variableSymbol);
        payer ??= new(variableSymbol);

        if (paymentReference is not null)
        {
            ManualPayment[] concurrent = payer.ManualPayments.Where(p => p.Reference == paymentReference).ToArray();
            foreach (ManualPayment c in concurrent)
                payer.ManualPayments.Remove(c);
        }

        payer.ManualPayments.Add(new(
            paymentReference ?? Guid.NewGuid().ToString(), 
            constantSymbol, 
            amountCzk, 
            dateTime, 
            description));

        await _payers.UpsertAync(payer);
    }

    public async Task OverrideBankPaymentAsync(ulong variableSymbol, string paymentReference, 
        ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol);

        BankPayment payment = payer.BankPayments.Single(p => p.Reference == paymentReference);
        payment.Overrides.ConstantSymbol = constantSymbol;
        payment.Overrides.DateTime = dateTime;
        payment.Overrides.Description = description;

        await _payers.UpsertAync(payer);
    }

    public async Task RemovePaymentAsync(ulong variableSymbol, string paymentReference)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol);
        
        switch (FilterOutPayment(payer, paymentReference, true)!)
        {
            case BankPayment bankPayment:
                bankPayment.Overrides.Removed = true;
                break;
            case ManualPayment manualPayment:
                payer.ManualPayments.Remove(manualPayment);
                break;
            default:
                throw new IndexOutOfRangeException();
        }

        await _payers.UpsertAync(payer);
    }

    public async Task RestorePaymentAsync(ulong variableSymbol, string paymentReference)
    {
        Payer payer = await _payers.GetRequiredAsync(variableSymbol);

        object? payment = FilterOutPayment(payer, paymentReference, false);
        if (payment is not BankPayment bankPayment)
            throw new InvalidOperationException("Je možné obnovit pouze bankovní plataby!");

        bankPayment.Overrides.Removed = false;

        await _payers.UpsertAync(payer);
    }

    private readonly IPayersDao _payers;

    private object? FilterOutPayment(Payer payer, string paymentReference, bool throwIfNotFound)
    {
        BankPayment? maybeBankPayment = payer.BankPayments.SingleOrDefault(p => p.Reference == paymentReference);
        if (maybeBankPayment is not null)
            return maybeBankPayment;

        ManualPayment? maybeManualPayment = payer.ManualPayments.SingleOrDefault(p => p.Reference == paymentReference);
        if (maybeManualPayment is not null)
            return maybeManualPayment;

        if (throwIfNotFound)
            throw new NullReferenceException(
                $"Pro poplatníka {payer.VariableSymbol} neexistuje platba {paymentReference}!");

        return null;
    }
}