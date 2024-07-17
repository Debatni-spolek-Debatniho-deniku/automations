using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.BankPayment;

public class BankPaymentViewModel : LayoutViewModel
{
    public ulong VariableSymbol { get; }

    public BankPaymentFormViewModel Payment { get; }

    public BankPaymentViewModel(ulong variableSymbol, Model.BankPayment payment) : base($"Bankovní platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        Payment = new(payment);
    }
}