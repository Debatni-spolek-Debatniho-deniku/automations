namespace DSDD.Automations.Payments.Model;

public class BankPayment
{
    public string Reference { get; }
    
    public string CounterpartyAccountNumber { get; }

    public ulong? ConstantSymbol { get; }

    public decimal AmountCzk { get; }

    public DateTime DateTime { get; }

    public string? Description { get; }

    public BankPaymentOverrides Overrides { get; }

    public BankPayment(string reference, string counterpartyAccountNumber, ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description, BankPaymentOverrides overrides)
    {
        Reference = reference;
        CounterpartyAccountNumber = counterpartyAccountNumber;
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
        Overrides = overrides;
    }
}