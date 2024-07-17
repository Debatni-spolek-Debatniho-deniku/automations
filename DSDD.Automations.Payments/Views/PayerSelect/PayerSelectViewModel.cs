namespace DSDD.Automations.Payments.Views.PayerSelect;

public class PayerSelectViewModel
{
    public ulong? VariableSymbol { get; }

    public PayerSelectViewModel(ulong? variableSymbol = null)
    {
        VariableSymbol = variableSymbol;
    }
}