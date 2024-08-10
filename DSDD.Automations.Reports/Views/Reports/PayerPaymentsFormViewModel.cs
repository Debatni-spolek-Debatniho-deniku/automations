namespace DSDD.Automations.Reports.Views.Reports;

public class PayerPaymentsFormViewModel
{
    public ulong VariableSymbol { get; }

    public ulong? ConstantSymbol { get; }

    public PayerPaymentsFormViewModel(ulong variableSymbol, ulong? constantSymbol)
    {
        VariableSymbol = variableSymbol;
        ConstantSymbol = constantSymbol;
    }
}