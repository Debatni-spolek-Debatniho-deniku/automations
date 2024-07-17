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

    public BankPaymentFormViewModel(Model.BankPayment payment)
        : this(payment.Overrides.ConstantSymbol, payment.Overrides.DateTime, payment.Overrides.Description)
    { }
}