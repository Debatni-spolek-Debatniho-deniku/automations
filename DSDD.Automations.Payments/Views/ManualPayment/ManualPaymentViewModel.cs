using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.ManualPayment;

public class ManualPaymentViewModel : LayoutViewModel
{
    public ulong VariableSymbol { get; }

    public ManualPaymentFormViewModel? Payment { get; }

    public ManualPaymentViewModel(ulong variableSymbol): base($"Nová manuální platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        Payment = null;
    }

    public ManualPaymentViewModel(ulong variableSymbol, Model.ManualPayment payment) : base($"Manuální platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        Payment = new(payment);
    }
}