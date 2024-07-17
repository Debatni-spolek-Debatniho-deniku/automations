using System.ComponentModel.DataAnnotations;
using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.Payer;

public class PayerViewModel : LayoutViewModel
{
    [Display(Name="Variabilní symbol")]
    public ulong VariableSymbol { get; }

    public IReadOnlyList<PaymentViewModel> Payments { get; }


    public PayerViewModel(ulong variableSymbol) : base($"Plátce s variabilním symbolem {variableSymbol}", null)
    {
        VariableSymbol = variableSymbol;
        Payments = Array.Empty<PaymentViewModel>();
    }

    public PayerViewModel(Model.Payer payer) : this(payer.VariableSymbol)
    {
        Payments = payer
            .BankPayments
            .Select(payment => new PaymentViewModel(payment))
            .Concat(payer
                .ManualPayment
                .Select(payment => new PaymentViewModel(payment)))
            .OrderByDescending(payment => payment.DateTime)
            .ToArray();
    }
}