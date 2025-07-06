using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;

namespace DSDD.Automations.Payments.Views.ManualPayment;

public class ManualPaymentFormViewModel
{
    public ulong? ConstantSymbol { get; }
    
    public decimal AmountCzk{ get; }
    
    public DateTime DateTime { get; }

    public string? Description { get; }
    
    public ManualPaymentFormViewModel(ulong? constantSymbol, decimal amountCzk, DateTime dateTime, string? description)
    {
        ConstantSymbol = constantSymbol;
        AmountCzk = amountCzk;
        DateTime = dateTime;
        Description = description;
    }

    public ManualPaymentFormViewModel(PayerManualPayment payment)
        : this(payment.ConstantSymbol, payment.AmountCzk, payment.DateTime, payment.Description)
    { }
}