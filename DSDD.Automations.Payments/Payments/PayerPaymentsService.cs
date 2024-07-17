using DSDD.Automations.Payments.Model;

namespace DSDD.Automations.Payments.Payments;

public class PayerPaymentsService: IPaymentsService
{
    public PayerPaymentsService(IPayersDao payers)
    {
        _payers = payers;
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

        bool found = false;
        
        if (payer.ManualPayments.SingleOrDefault(p => p.Reference == paymentReference) is { } manualPayment)
        {
            found = true;
            payer.ManualPayments.Remove(manualPayment);
        }

        if (payer.BankPayments.SingleOrDefault(p => p.Reference == paymentReference) is {} bankPayment)
        {
            found = true;
            bankPayment.Overrides.Hidden = true;
        }

        if (!found)
            throw new IndexOutOfRangeException(
                $"Platba {paymentReference} nebyla nalezena u poplatníka {variableSymbol}!");

        await _payers.UpsertAync(payer);
    }

    private readonly IPayersDao _payers;
}