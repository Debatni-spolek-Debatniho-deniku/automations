namespace DSDD.Automations.Payments.Views.BankPayment;

public class BankPaymentFormViewModel
{
    public ulong? ConstantSymbol { get; }
    
    public DateTime? DateTime { get; }

    public string? Description { get; }
    
    public BankPaymentFormViewModel(ulong? constantSymbol, DateTime? dateTime, string? description)
    {
        ConstantSymbol = constantSymbol;
        DateTime = dateTime;
        Description = description;
    }

    public BankPaymentFormViewModel(Model.BankPaymentOverrides overrides)
        : this(overrides.ConstantSymbol, overrides.DateTime, overrides.Description)
    { }
}