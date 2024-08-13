using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Model;

public class BankPaymentOverrides
{
    public bool Removed { get; set; }
    
    public ulong? ConstantSymbol { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Description { get; set; }
    
    public BankPaymentOverrides(bool removed, ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Removed = removed;
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }
}