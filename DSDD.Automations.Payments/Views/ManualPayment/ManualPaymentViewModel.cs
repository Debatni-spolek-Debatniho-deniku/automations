using DSDD.Automations.Payments.Persistence.Abstractions.Model.Payers;
using DSDD.Automations.Payments.Views.Shared;

namespace DSDD.Automations.Payments.Views.ManualPayment;

public class ManualPaymentViewModel : LayoutViewModel
{
    public ulong VariableSymbol { get; }

    public string? MemberName { get; }

    public ManualPaymentFormViewModel? Payment { get; }

    public ManualPaymentViewModel(ulong variableSymbol, string? memberName): base($"Nová manuální platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        MemberName = memberName;
        Payment = null;
    }

    public ManualPaymentViewModel(ulong variableSymbol, PayerManualPayment payment, string? memberName) : base($"Manuální platba poplatníka {variableSymbol}")
    {
        VariableSymbol = variableSymbol;
        MemberName = memberName;
        Payment = new(payment);
    }
}