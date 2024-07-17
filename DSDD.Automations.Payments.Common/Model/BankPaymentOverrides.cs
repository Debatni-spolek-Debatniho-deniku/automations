namespace DSDD.Automations.Payments.Model;

public class BankPaymentOverrides
{
    public bool Hidden { get; set; }
    
    public ulong? ConstantSymbol { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Description { get; set; }

    public BankPaymentOverrides(bool hidden, ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Hidden = hidden;
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }
}