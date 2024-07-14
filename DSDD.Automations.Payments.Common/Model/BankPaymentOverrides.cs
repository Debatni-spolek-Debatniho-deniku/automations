namespace DSDD.Automations.Payments.Model;

public class BankPaymentOverrides
{
    public bool Hidden { get; }

    public string ConstantSymbol { get; }

    public DateTime Date { get; }

    public BankPaymentOverrides(bool hidden, string constantSymbol, DateTime date)
    {
        Hidden = hidden;
        ConstantSymbol = constantSymbol;
        Date = date;
    }
}