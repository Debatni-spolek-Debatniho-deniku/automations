using DSDD.Automations.Payments.Views.Layout;

namespace DSDD.Automations.Payments.Views.ManualPayment;

public class ManualPaymentViewModel : LayoutViewModel
{
    public ulong VariableSymbol { get; }

    public ManualPaymentFormViewModel Payment { get; }

    public ManualPaymentViewModel(ulong variableSymbol, Exception? exception = null): base($"Nová manuální platba poplatníka {variableSymbol}", null)
    {
        VariableSymbol = variableSymbol;
        Payment = new();
    }

    public ManualPaymentViewModel(ulong variableSymbol, ManualPaymentFormViewModel payment, Exception exception): this(variableSymbol)
    {

    }

    public ManualPaymentViewModel(ulong variableSymbol, Model.ManualPayment payment, Exception? exception = null) : base($"Manuální platba poplatníka {variableSymbol}", exception)
    {
        VariableSymbol = variableSymbol;
        Payment = new(payment);
    }
}