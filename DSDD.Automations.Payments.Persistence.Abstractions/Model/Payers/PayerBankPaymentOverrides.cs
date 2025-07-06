namespace DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

public class PayerBankPaymentOverrides
{
    public bool Removed { get; set; }
    
    public ulong? ConstantSymbol { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Description { get; set; }
    
    public PayerBankPaymentOverrides(bool removed, ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        Removed = removed;
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }
}