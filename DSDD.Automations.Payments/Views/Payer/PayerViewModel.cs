using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Payer;

public class PayerViewModel : LayoutViewModel
{
    public ulong VariableSymbol { get; }

    public IReadOnlyList<PaymentViewModel> Payments { get; }
    
    public PayerViewModel(ulong variableSymbol) : base($"Poplatník s variabilním symbolem {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        Payments = Array.Empty<PaymentViewModel>();
    }

    public PayerViewModel(Model.Payer payer) : this(payer.VariableSymbol)
    {
        Payments = payer
            .BankPayments
            .Select(payment => new PaymentViewModel(payer.VariableSymbol, payment))
            .Concat(payer
                .ManualPayments
                .Select(payment => new PaymentViewModel(payer.VariableSymbol, payment)))
            .OrderByDescending(payment => payment.DateTime)
            .ToArray();
    }
}