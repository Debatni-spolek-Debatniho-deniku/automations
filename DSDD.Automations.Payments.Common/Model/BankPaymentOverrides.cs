using Newtonsoft.Json;

namespace DSDD.Automations.Payments.Model;

public class BankPaymentOverrides
{
    public bool Removed { get; set; }
    
    public ulong? ConstantSymbol { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Description { get; set; }

    [JsonIgnore]
    public bool Overriden => ConstantSymbol is not null || DateTime is not null || Description is not null;

    public BankPaymentOverrides(bool removed, ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Removed = removed;
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }
}