namespace DSDD.Automations.Payments.Model;

public class BankPaymentOverrides
{
    public bool Hidden { get; }
    
    public ulong? ConstantSymbol { get; }

    public DateTime? DateTime { get; }

    public string? Description { get; }

    public BankPaymentOverrides(bool hidden, ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Hidden = hidden;
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }
}