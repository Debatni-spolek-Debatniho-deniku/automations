namespace DSDD.Automations.Payments.Model;

public class BankPayment
{
    public string Reference { get; }
    
    public string OriginatorAccountNumber { get; }

    public string ConstantSymbol { get; }

    public Amount Amount { get; }

    public DateTime Date { get; }

    public string Description { get; }

    public BankPaymentOverrides Overrides { get; }

    public BankPayment(string reference, string originatorAccountNumber, string constantSymbol, Amount amount, DateTime date, string description, BankPaymentOverrides overrides)
    {
        Reference = reference;
        OriginatorAccountNumber = originatorAccountNumber;
        ConstantSymbol = constantSymbol;
        Amount = amount;
        Date = date;
        Description = description;
        Overrides = overrides;
    }
}